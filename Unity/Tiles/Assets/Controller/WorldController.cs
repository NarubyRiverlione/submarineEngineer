using Submarine.Model;
using UnityEngine;
using UnityEngine.UI;
using System;

public class WorldController : MonoBehaviour {

	// instance to become a singleton
	public static WorldController instance { get; private set; }

	// Model
	public Sub mySub { get; private set; }


	// Tile Sprites
	public Sprite Tile_Empty;
	public Sprite Tile_Transparent;
	public Sprite Tile_Unknown;
	public Sprite Tile_Bridge;
	public Sprite Tile_EngineRoom;
	public Sprite Tile_Generator;
	public Sprite Tile_Battery;
	public Sprite Tile_Gallery;
	public Sprite Tile_Cabin;
	public Sprite Tile_Bunks;
	public Sprite Tile_Conn;
	public Sprite Tile_Sonar;
	public Sprite Tile_RadioRoom;
	public Sprite Tile_FuelTank;
	public Sprite Tile_StorageRoom;
	public Sprite Tile_TorpedoRoom;
	public Sprite Tile_Stairs;
	public Sprite Tile_Balasttank;
	public Sprite Tile_PumpRoom;
	public Sprite Tile_Escapehatch;

	// Warning Sprites
	public Sprite Warning_ToSmall;
	public Sprite Waring_NoResources;

	// Wall sprite sheet (private, loaded in Start)
	Sprite[] WallSpriteSheet;
	Sprite[] PipeSpriteSheet;
	Sprite[] FuelSpriteSheet;
	Sprite[] WaterSpriteSheet;
	Sprite[] CableSpriteSheet;
	Sprite[] ElectricitySpriteSheet;

	public Sprite ShaftSprite;
	public Sprite ShaftContentSprite;

	// Connections
	public Sprite Sprite_PipeConnection;

	// UI Panels
	public GameObject Panel_Resources;
	public GameObject Panel_Legenda;
	public GameObject Panel_DesignValidation;

	// FileBrowser Browser
	public UI_FileBrowser _uiFB;
	public string filePath = null;
	private bool chooseFilePathNow = false;
	private bool Loading = false;
	string saveDir = "Saves";



	// Use this for initialization
	void Start () {
		instance = this;
		mySub = new Sub (); // will set dimensions so game objects for each tile can be created
		//Debug.Log ("Sub created with length:" + mySub.lengthOfSub + " & height " + mySub.heightOfSub);
	
		// loading Wall sprites from sheet
		WallSpriteSheet = Resources.LoadAll<Sprite> ("Walls");
		// loading Pipes sprites from sheet
		PipeSpriteSheet = Resources.LoadAll<Sprite> ("Pipes_Empty");
		// loading Fuel sprites from sheet
		FuelSpriteSheet = Resources.LoadAll<Sprite> ("FuelPipe_Content");
		// TODO loading Water sprites 
		WaterSpriteSheet = Resources.LoadAll<Sprite> ("Water_Content");
		// TODO loading electricity
		ElectricitySpriteSheet = Resources.LoadAll<Sprite> ("Electricity_Content");
		// TODO loading cables
		CableSpriteSheet = Resources.LoadAll<Sprite> ("Cables_Empty");

		CreateAllTileGameObjects ();
		mySub.SetOutlines (); 			// create fixed rooms (bridge), set tile outside submarine outline as unavailable
		ShowAllTilesViaCallback ();		// now show tiles (needed for empty tiles)

//		#if DEBUG
//		// create fuel tank
//		mySub.AddTileToRoom (10, 1, RoomType.FuelTank);
//		mySub.AddTileToRoom (11, 1, RoomType.FuelTank);
//		mySub.AddTileToRoom (12, 1, RoomType.FuelTank);
//		mySub.AddTileToRoom (13, 1, RoomType.FuelTank);
//		mySub.AddTileToRoom (10, 3, RoomType.FuelTank);
//		mySub.AddTileToRoom (11, 3, RoomType.FuelTank);
//		mySub.AddTileToRoom (12, 3, RoomType.FuelTank);
//		mySub.AddTileToRoom (13, 3, RoomType.FuelTank);
//		mySub.AddTileToRoom (10, 2, RoomType.FuelTank);
//		mySub.AddTileToRoom (11, 2, RoomType.FuelTank);
//		mySub.AddTileToRoom (12, 2, RoomType.FuelTank);
//		mySub.AddTileToRoom (13, 2, RoomType.FuelTank);
//
//
//		#endif
	}
	
	// Update is called once per frame
	void Update () {

		UpdateCrewFreeSpaces ();

		// if a filepath is requited (chooseFilePathNow==true) and FileBrowserWindow isn't visible (any more)
		// the a file is chosen or cancel is pressed
		if (!_uiFB.FileBrowserWindow.activeSelf && chooseFilePathNow) {
			// show other panels for view
			Panel_Resources.SetActive (true);
			Panel_Legenda.SetActive (true);
			Panel_DesignValidation.SetActive (true);
			_uiFB.FileBrowserWindow.transform.parent.GetComponent<Image> ().raycastTarget = true;

			filePath = _uiFB.GetPath ();
			chooseFilePathNow = false;
			if (Loading)
				LoadingSub ();
			else // loading false = saving
				SavingSub ();
			
		}

	}

	#region Update Graphics

	void CreateAllTileGameObjects () {
		// Create GameObject for each Tile
		for (int x = 0; x < mySub.lengthOfSub; x++) {
			for (int y = 0; y < mySub.heightOfSub; y++) {
				Tile newTile = mySub.GetTileAt (x, y);
				GameObject newTileSprite = new GameObject ();
				// set name of game object to see in Hierarchy
				newTileSprite.name = "Tile_" + x + "/" + y;
				// set WorldController as parent
				newTileSprite.transform.SetParent (this.transform);
				// set X, Y of game object
				newTileSprite.transform.position = new Vector2 (newTile.X, newTile.Y);
				// add Sprite Renderer component
				newTileSprite.AddComponent<SpriteRenderer> ();
				// when action of the title is called, update the sprite
				newTile.TileChangedActions += (tile => {
					UpdateTileSprite (newTile, newTileSprite);
				});
				// Tile changed = room changes = rescources changed
				// update Resource Panel
				newTile.TileChangedActions += ((tile) => {
					UpdateResourceLabels ();
				});
				// Update Design Validation Panel
				newTile.TileChangedActions += ((tile) => {
					UpdateDesignValidation ();
				});

			}
		}
	}

	void RemoveAllTileGameObjects () {
		for (int x = 0; x < mySub.lengthOfSub; x++) {
			for (int y = 0; y < mySub.heightOfSub; y++) {
				string name = "Tile_" + x + "/" + y;
				GameObject tile_gameObject = GameObject.Find (name);
				Destroy (tile_gameObject); // destroys also all child game objects
			}
		}
	}
		
	// only call this in Start and LoadingSub, changes are being done via Callback Action
	void ShowAllTilesViaCallback () {
		for (int x = 0; x < mySub.lengthOfSub; x++) {
			for (int y = 0; y < mySub.heightOfSub; y++) {
				Tile checkTile = mySub.GetTileAt (x, y);
				if (checkTile.TileChangedActions != null)
					checkTile.TileChangedActions (checkTile);
				//				else
				//					Debug.LogError ("ERROR no action found for tile (" + x + "," + y + "), roomID:" + checkTile.RoomID);
			}
		}
	}

	// get Tile at x,y in World
	public Tile GetTileAtWorldCoordinates (Vector3 coord) {
		int x = Mathf.FloorToInt (coord.x);
		int y = Mathf.FloorToInt (coord.y);

		return mySub != null ? mySub.GetTileAt (x, y) : null;
	}

	// Update Tile (RoomType, Warnings, Wall)
	void UpdateTileSprite (Tile showTile, GameObject tile_GameObj) {
		SpriteRenderer tile_renderer = tile_GameObj.GetComponent<SpriteRenderer> ();
		#region Room Type
		// Update Room Type Sprite
		switch (mySub.GetRoomTypeOfTile (showTile)) {
			case RoomType.Empty:
				tile_renderer.sprite = Tile_Empty;
				break;

			case RoomType.Stairs:
				tile_renderer.sprite = Tile_Stairs;
				break;
			case RoomType.EscapeHatch:
				tile_renderer.sprite = Tile_Escapehatch;
				break;
			
			case RoomType.Propellor:
				tile_renderer.sprite = Tile_Bridge;
				break;
			case RoomType.EngineRoom:
				tile_renderer.sprite = Tile_EngineRoom;
				break;
			case RoomType.FuelTank:
				tile_renderer.sprite = Tile_FuelTank;
				break;
			case RoomType.Generator:
				tile_renderer.sprite = Tile_Generator;
				break;
			case RoomType.Battery:
				tile_renderer.sprite = Tile_Battery;
				break;

			case RoomType.Gallery:
				tile_renderer.sprite = Tile_Gallery;
				break;
			case RoomType.Cabin:
				tile_renderer.sprite = Tile_Cabin;
				break;
			case RoomType.Bunks:
				tile_renderer.sprite = Tile_Bunks;
				break;
			case RoomType.StorageRoom:
				tile_renderer.sprite = Tile_StorageRoom;
				break;

			case RoomType.Bridge:
				tile_renderer.sprite = Tile_Bridge;
				break;
			case RoomType.Conn:
				tile_renderer.sprite = Tile_Conn;
				break;
			case RoomType.Sonar:
				tile_renderer.sprite = Tile_Sonar;
				break;
			case RoomType.RadioRoom:
				tile_renderer.sprite = Tile_RadioRoom;
				break;

			case RoomType.BalastTank:
				tile_renderer.sprite = Tile_Balasttank;
				break;
			case RoomType.PumpRoom:
				tile_renderer.sprite = Tile_PumpRoom;
				break;

		
			case RoomType.TorpedoRoom:
				tile_renderer.sprite = Tile_TorpedoRoom;
				break;
			default:
				tile_renderer.sprite = Tile_Unknown;
				break;
		}

		// cannot be build on = outside sub = show transparent
		if (showTile != null && !showTile.canContainRoom) {
			tile_renderer.sprite = Tile_Transparent;
		}
		#endregion

		#region Warnings
		// check for warnings (valid layout)
		GameObject tileWarning_GameObj = GameObject.Find ("Tile_Warning_" + tile_GameObj.transform.position.x + "/" + tile_GameObj.transform.position.y);
		if (showTile.RoomID != 0) {
			if (tileWarning_GameObj == null) {
				// create warning game object now		
				tileWarning_GameObj = new GameObject ();
				// set name of game object to see in Hierarchy
				tileWarning_GameObj.name = "Tile_Warning_" + tile_GameObj.transform.position.x + "/" + tile_GameObj.transform.position.y;
				// set parent of warning GameObject to the Title game object
				tileWarning_GameObj.transform.SetParent (tile_GameObj.transform);
				// set X, Y of game object
				tileWarning_GameObj.transform.position = new Vector2 (tile_GameObj.transform.position.x, tile_GameObj.transform.position.y);
				// add Sprite component
				tileWarning_GameObj.AddComponent<SpriteRenderer> ();	
				// show above Title = on the Tile_Warning sorting layer  						
				tileWarning_GameObj.GetComponent<SpriteRenderer> ().sortingLayerName = "Warnings";
			}
			// now it's sure the Tile_Warning_ game object exist, update (or set) it's sprite
			// Show warning if room cannot operate because lake of resources
			bool resourceAvailable = mySub.IsTilePartOfRoomWithResources (showTile);
			SpriteRenderer tileWarning_renderer = tileWarning_GameObj.GetComponent<SpriteRenderer> ();
			if (!resourceAvailable)
				tileWarning_renderer.sprite = Waring_NoResources;
			// Show warning if room layout is invalid
			bool layoutValid = mySub.IsTilePartOfValidRoomLayout (showTile);
			if (!layoutValid)
				tileWarning_renderer.sprite = Warning_ToSmall;
			// Show no warnings (remove previous) if all is fine
			if (resourceAvailable && layoutValid)
				tileWarning_renderer.sprite = Tile_Transparent;
			
		}
		else { // roomID = 0,  don't remove Warning game object but set sprite to transparent (removing gives strange behavior)
			if (tileWarning_GameObj != null)
				tileWarning_GameObj.GetComponent<SpriteRenderer> ().sprite = Tile_Transparent;
		}
		#endregion

		#region Wall
		// add wall type
		GameObject tileWall_GameObj = GameObject.Find ("Wall_" + tile_GameObj.transform.position.x + "/" + tile_GameObj.transform.position.y);
		if (showTile.RoomID != 0) {
			// only create GameObject when need to show a wall
			if (tileWall_GameObj == null) { 
				// add Wall now		
				tileWall_GameObj = new GameObject ();
				// set name of game object to see in Hierarchy
				tileWall_GameObj.name = "Wall_" + tile_GameObj.transform.position.x + "/" + tile_GameObj.transform.position.y;
				// set parent of warning GameObject to the Title game object
				tileWall_GameObj.transform.SetParent (tile_GameObj.transform);
				// set X, Y of game object
				tileWall_GameObj.transform.position = new Vector2 (tile_GameObj.transform.position.x, tile_GameObj.transform.position.y);
				// add Sprite Renderer
				tileWall_GameObj.AddComponent<SpriteRenderer> ();

			}
			// now it's sure the Wall game object exist, update (or set) it's sprite
			SpriteRenderer tileWall_renderer = tileWall_GameObj.GetComponent<SpriteRenderer> ();
			//set sprite from wall sprite sheet
			//Debug.Log ("For (" + showTile.X + "," + showTile.Y + ") show wall type " + showTile.WallType);
			tileWall_renderer.sprite = WallSpriteSheet [showTile.WallType];
			// show above Title = on the Tile_Warning sorting layer  						
			tileWall_renderer.sortingLayerName = "Walls";
		}
		else { // roomID = 0,  don't remove Wall game object but set it to wall type 15 = show no walls (removing wall type gives strange behavior)
			if (tileWall_GameObj != null) {
				SpriteRenderer tileWall_renderer = tileWall_GameObj.GetComponent<SpriteRenderer> ();
				//set sprite from wall sprite sheet
				//Debug.Log ("Remove all walls for  (" + showTile.X + "," + showTile.Y + ")");
				tileWall_renderer.sprite = WallSpriteSheet [15]; // no room = no walls
			}
			
		}
		#endregion

		#region Pieces
	
		foreach (Piece piece in showTile.PiecesOnTile) {
			string namePiece_obj = "Piece_" + showTile.PiecesOnTile.IndexOf (piece) + "__"
			                       + tile_GameObj.transform.position.x + "/" + tile_GameObj.transform.position.y;		
			GameObject piece_GameObj = GameObject.Find (namePiece_obj);

			string namePieceContent_GameObj = "PiecesContent_" + showTile.PiecesOnTile.IndexOf (piece) + "__" + tile_GameObj.transform.position.x + "/" + tile_GameObj.transform.position.y;
			GameObject pieceContent_GameObj = GameObject.Find (namePieceContent_GameObj);

			if (mySub.ResourceCarriers.ContainsKey (piece.carrierID)) {
				Carrier carrierOfPiece = mySub.ResourceCarriers [piece.carrierID];
				// Show piece outline
				if (piece_GameObj == null) { 
					// add piece now		
					piece_GameObj = new GameObject ();
					// set name of game object to see in Hierarchy
					piece_GameObj.name = namePiece_obj;
					// set parent of warning GameObject to the Title game object
					piece_GameObj.transform.SetParent (tile_GameObj.transform);
					// set X, Y of game object
					piece_GameObj.transform.position = new Vector2 (tile_GameObj.transform.position.x, tile_GameObj.transform.position.y);
					// add Sprite Renderer
					piece_GameObj.AddComponent<SpriteRenderer> ();
				}
				// now it's sure the Pieces game object exist, update (or set) it's sprite
				SpriteRenderer render = piece_GameObj.GetComponent<SpriteRenderer> ();
				//set sprite from Pieces sprite sheet
				//Debug.Log ("For (" + showTile.X + "," + showTile.Y + ") show Pipe sprite " + pipe.NeighboreCount);
				switch (carrierOfPiece.UnitOfContent) {
					case Units.liters_fuel:
						render.sprite = PipeSpriteSheet [piece.NeighboreCount];
						break;
					case Units.liters_pump:
						render.sprite = PipeSpriteSheet [piece.NeighboreCount];
						break;
					case Units.pks:
						render.sprite = ShaftSprite;
						break;
					case Units.MWs:
						render.sprite = CableSpriteSheet [piece.NeighboreCount];
						break;
					default:
						render.sprite = Tile_Transparent;
						break;

				}
				// show above Title = on the Pieces sorting layer  						
				render.sortingLayerName = "Pieces";
				render.sortingOrder = 0;


//				//Show Connection
//				if (pieceConnection_GameObj == null) { 
//					// add Connection GameObject now		
//					pieceConnection_GameObj = new GameObject ();
//					// set name of game object to see in Hierarchy
//					pieceConnection_GameObj.name = "PiecesConnection_" + tile_GameObj.transform.position.x + "/" + tile_GameObj.transform.position.y;
//					// set parent of warning GameObject to the Pieces game object
//					pieceConnection_GameObj.transform.SetParent (piece_GameObj.transform);
//					// set X, Y of game object
//					pieceConnection_GameObj.transform.position = new Vector2 (tile_GameObj.transform.position.x, tile_GameObj.transform.position.y);
//					// add Sprite Renderer
//					pieceConnection_GameObj.AddComponent<SpriteRenderer> ();
//				}
//				// now it's sure the PiecesConnection game object exist, update (or set) it's sprite
//				SpriteRenderer renderConnection = pieceConnection_GameObj.GetComponent<SpriteRenderer> ();
//				//set sprite from Pieces sprite sheet
//				//Debug.Log ("For (" + showTile.X + "," + showTile.Y + ") show Pipe CONNECTION sprite " + pipe.IsConnection);
//				if (piece.IsConnection) {
//					switch (carrierOfPiece.UnitOfContent) {
//						case Units.liters_fuel:
//							renderConnection.sprite = Sprite_PipeConnection;
//							break;
//						case Units.liters_pump:
//							renderConnection.sprite = Sprite_PipeConnection;
//							break;
//						case Units.pks:
//							renderConnection.sprite = Sprite_PipeConnection;
//							break;
//						case Units.MWs:
//							renderConnection.sprite = Sprite_PipeConnection;
//							break;
//					}
//				}
//				else
//					renderConnection.sprite = Tile_Transparent;
//				// show above piece outline & conten 					
//				renderConnection.sortingLayerName = "Pieces";
//				renderConnection.sortingOrder = 10;


				//Show Content
				if (pieceContent_GameObj == null) { 
					// add Contet GameObject now		
					pieceContent_GameObj = new GameObject ();
					// set name of game object to see in Hierarchy
					pieceContent_GameObj.name = namePieceContent_GameObj;
					// set parent of warning GameObject to the Pieces game object
					pieceContent_GameObj.transform.SetParent (piece_GameObj.transform);
					// set X, Y of game object
					pieceContent_GameObj.transform.position = new Vector2 (tile_GameObj.transform.position.x, tile_GameObj.transform.position.y);
					// add Sprite Renderer
					pieceContent_GameObj.AddComponent<SpriteRenderer> ();
				}
				// now it's sure the Pieces Content game object exist, update (or set) it's sprite
				SpriteRenderer renderContent = pieceContent_GameObj.GetComponent<SpriteRenderer> ();
				//set sprite from Pieces sprite sheet
				//Debug.Log ("For (" + showTile.X + "," + showTile.Y + ") show Pipe CONTENT sprite " + pipe.IsConnection);

				// this update Tile will be called when adding an pipe too the title, before the pipe is added to a carrier
				if (carrierOfPiece.Content > 0) {

					switch (carrierOfPiece.UnitOfContent) {
						case Units.liters_fuel:
							renderContent.sprite = FuelSpriteSheet [piece.NeighboreCount];
							break;
						case Units.liters_pump:
							//TODO: change to water content spitesheet
							renderContent.sprite = WaterSpriteSheet [piece.NeighboreCount];
							break;
						case Units.pks:
							renderContent.sprite = ShaftContentSprite;
							break;
						case Units.MWs:
							renderContent.sprite = ElectricitySpriteSheet [piece.NeighboreCount];
							break;
					}
				}
				else
					renderContent.sprite = Tile_Transparent;
				// show above Title = on the Pieces sorting layer  						
				renderContent.sortingLayerName = "Pieces";
				renderContent.sortingOrder = 2;

			}

			if (piece.Type == PieceType.None) {
				if (piece_GameObj != null)
					piece_GameObj.GetComponent<SpriteRenderer> ().sprite = Tile_Transparent; 
			
				if (pieceContent_GameObj != null)
					pieceContent_GameObj.GetComponent<SpriteRenderer> ().sprite = Tile_Transparent; 
			}
		}
		#endregion
	}

	//  Update all UI panels (too be called from mouse controller)
	public void UpdateUIpanels () {
		UpdateResourceLabels ();
		UpdateDesignValidation ();
		UpdateCrewFreeSpaces ();
	}

	// Update UI Resources
	void UpdateResourceLabels () {
		foreach (Units resourceUnit in Enum.GetValues(typeof(Units))) {
			if (resourceUnit != Units.None) { 
				GameObject resourceGameObject = GameObject.Find ("Resource_" + resourceUnit);
				if (resourceGameObject != null) { // some Units are showed in UI Design Validation, not here
					// AVAILABLE
					int outputCount = 0;

					if (Resource.isCrewType (resourceUnit))
						// get crew count
						outputCount = mySub.AmountOfCrewType (resourceUnit);
					else
						// get non-crew count
						outputCount = mySub.GetAllOutputOfUnit (resourceUnit);
					
					// NEEDED
					int neededCount = mySub.GetAllNeededResourcesOfUnit (resourceUnit);

					// show text (available / needed)
					Text resourceText = resourceGameObject.GetComponent<Text> ();
					resourceText.text = neededCount.ToString () + " / " + outputCount.ToString ();
					// not enough resources = show text in red
					resourceText.color = neededCount > outputCount ? Color.red : Color.white;
				}
//				else {
//					Debug.Log ("No resource label for " + resourceUnit);
//				}
			}
		}
	}

	// Update UI Design Validation
	void UpdateDesignValidation () {
		string validationCriteria;
		validationCriteria = "Ops";
		SetOrResetValidation (validationCriteria, mySub.ValidateOps ());

		validationCriteria = "Crew";
		SetOrResetValidation (validationCriteria, mySub.ValidateCrew ());

		validationCriteria = "Weapons";
		SetOrResetValidation (validationCriteria, mySub.ValidateWeapons ());

		validationCriteria = "Propulsion";
		SetOrResetValidation (validationCriteria, mySub.ValidatePropulsion ());

	}
	// update 1 Validation Criteria
	void SetOrResetValidation (string validationCriteria, bool ok) {
		GameObject findCheckbox = GameObject.Find ("Toggle_" + validationCriteria);
		if (findCheckbox != null) {
			findCheckbox.GetComponent<Toggle> ().isOn = ok;
		}
		else {
			Debug.Log ("No validation checkbox for " + validationCriteria);
		}
	}

	// update free slots in Add Crew Panel
	void UpdateCrewFreeSpaces () {
		if (GameObject.Find ("ScrollView_CrewButtons") != null && mySub != null) { // only when Crew panel is shown and sub is fully loaded
			GameObject.Find ("Officer place").GetComponent<Text> ().text = mySub.SpacesForOfficers.ToString ();

			GameObject.Find ("Cook place").GetComponent<Text> ().text = mySub.SpacesForCooks.ToString ();

			GameObject.Find ("SonarMan place").GetComponent<Text> ().text = mySub.SpacesForEnlisted.ToString ();
			GameObject.Find ("RadioMan place").GetComponent<Text> ().text = mySub.SpacesForEnlisted.ToString ();
			GameObject.Find ("TorpedoMan place").GetComponent<Text> ().text = mySub.SpacesForEnlisted.ToString ();
			GameObject.Find ("Engineer place").GetComponent<Text> ().text = mySub.SpacesForEnlisted.ToString ();
			GameObject.Find ("Watchstanders place").GetComponent<Text> ().text = mySub.SpacesForEnlisted.ToString ();

			// enable / disable Add buttons
			Button button; 
			button = GameObject.Find ("Officer_+").GetComponent<Button> ();
			button.interactable = mySub.SpacesForOfficers > 0;

			button = GameObject.Find ("Cook_+").GetComponent<Button> ();
			button.interactable = mySub.SpacesForCooks > 0;

			button = GameObject.Find ("Engineer+").GetComponent<Button> ();
			button.interactable = mySub.SpacesForEnlisted > 0;
			button = GameObject.Find ("SonarMan_+").GetComponent<Button> ();
			button.interactable = mySub.SpacesForEnlisted > 0;
			button = GameObject.Find ("RadioMan_+").GetComponent<Button> ();
			button.interactable = mySub.SpacesForEnlisted > 0;
			button = GameObject.Find ("TorpedoMan_+").GetComponent<Button> ();
			button.interactable = mySub.SpacesForEnlisted > 0;
			button = GameObject.Find ("Watchstanders_+").GetComponent<Button> ();
			button.interactable = mySub.SpacesForEnlisted > 0;

			// enable / disable Remove buttons
			button = GameObject.Find ("Officer_-").GetComponent<Button> ();
			button.interactable = mySub.AmountOfOfficers > 0;

			button = GameObject.Find ("Cook_-").GetComponent<Button> ();
			button.interactable = mySub.AmountOfCooks > 0;

			button = GameObject.Find ("Engineer-").GetComponent<Button> ();
			button.interactable = mySub.AmountOfEnlisted () > 0;
			button = GameObject.Find ("SonarMan_-").GetComponent<Button> ();
			button.interactable = mySub.AmountOfEnlisted () > 0;
			button = GameObject.Find ("RadioMan_-").GetComponent<Button> ();
			button.interactable = mySub.AmountOfEnlisted () > 0;
			button = GameObject.Find ("TorpedoMan_-").GetComponent<Button> ();
			button.interactable = mySub.AmountOfEnlisted () > 0;
			button = GameObject.Find ("Watchstanders_-").GetComponent<Button> ();
			button.interactable = mySub.AmountOfEnlisted () > 0;
		
		}
	}

	#endregion


	#region Load / Save

	// Get file path
	public void GetFilePath (bool loading) {
		// remove other panels for view
		Panel_Resources.SetActive (false);
		Panel_Legenda.SetActive (false);
		Panel_DesignValidation.SetActive (false);

        
		_uiFB.Open (saveDir, loading);
		//_uiFB.FileBrowserWindow.GetComponentInParent<Image>().raycastTarget = false;
		_uiFB.FileBrowserWindow.transform.parent.GetComponent<Image> ().raycastTarget = false;
		chooseFilePathNow = true; 
		Loading = loading;
	}

	// Loading sub
	private void LoadingSub () {
		if (filePath != null) {
			Loading = false;
			// destroy all Tile game objects (and the wall, warning,.. children)
			// (maybe new loaded sub has other dimensions)
			RemoveAllTileGameObjects ();
			// load new Sub 
			mySub.Load (filePath);
			// add all tiles game objects 
			CreateAllTileGameObjects ();     // also subscript too the  .TileChangedActions with UpdateTileSprite 
			// Show Tiles AFTER they are ALL created: because else showing a tile can call it's not already created neighbor to updates it's warning or wall
			ShowAllTilesViaCallback ();
		}
	}
        

	// Saving sub
	private void SavingSub () {
		if (filePath != null) {
			mySub.Save (filePath);
		}
	}

	#endregion

	public void QuitGame () {
		Application.Quit ();
	}


	public void AddCrew (string typeOfCrew) {
		Units crewType = (Units)Enum.Parse (typeof(Units), typeOfCrew);
		mySub.AddCrew (crewType);
	}

	public void RemoveCrew (string typeOfCrew) {
		Units crewType = (Units)Enum.Parse (typeof(Units), typeOfCrew);
		mySub.RemoveCrew (crewType);
	}

}
