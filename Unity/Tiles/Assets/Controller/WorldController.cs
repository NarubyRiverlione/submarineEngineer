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

	// UI Panels
	public GameObject Panel_Resources;
	public GameObject Panel_Legenda;
	public GameObject Panel_DesignValidation;

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

		CreateAllTileGameObjects ();
		mySub.SetOutlines (); 			// create fixed rooms (bridge), set tile outside submarine outline as unavailable
		ShowAllTilesViaCallback ();		// now show tiles (needed for empty tiles)
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
	void UpdateTileSprite (Tile showTile, GameObject gameObjectOfTitle) {
		SpriteRenderer renderer = gameObjectOfTitle.GetComponent<SpriteRenderer> ();
		#region Room Type
		// Update Room Type Sprite
		switch (mySub.GetRoomTypeOfTile (showTile)) {
			case RoomType.Empty:
				renderer.sprite = Tile_Empty;
				break;

			case RoomType.Stairs:
				renderer.sprite = Tile_Stairs;
				break;
			case RoomType.EscapeHatch:
				renderer.sprite = Tile_Escapehatch;
				break;
			
			case RoomType.EngineRoom:
				renderer.sprite = Tile_EngineRoom;
				break;
			case RoomType.FuelTank:
				renderer.sprite = Tile_FuelTank;
				break;
			case RoomType.Generator:
				renderer.sprite = Tile_Generator;
				break;
			case RoomType.Battery:
				renderer.sprite = Tile_Battery;
				break;

			case RoomType.Gallery:
				renderer.sprite = Tile_Gallery;
				break;
			case RoomType.Cabin:
				renderer.sprite = Tile_Cabin;
				break;
			case RoomType.Bunks:
				renderer.sprite = Tile_Bunks;
				break;
			case RoomType.StorageRoom:
				renderer.sprite = Tile_StorageRoom;
				break;

			case RoomType.Bridge:
				renderer.sprite = Tile_Bridge;
				break;
			case RoomType.Conn:
				renderer.sprite = Tile_Conn;
				break;
			case RoomType.Sonar:
				renderer.sprite = Tile_Sonar;
				break;
			case RoomType.RadioRoom:
				renderer.sprite = Tile_RadioRoom;
				break;

			case RoomType.BalastTank:
				renderer.sprite = Tile_Balasttank;
				break;
			case RoomType.PumpRoom:
				renderer.sprite = Tile_PumpRoom;
				break;

		
			case RoomType.TorpedoRoom:
				renderer.sprite = Tile_TorpedoRoom;
				break;
			default:
				renderer.sprite = Tile_Unknown;
				break;
		}

		// cannot be build on = outside sub = show transparent
		if (showTile != null && !showTile.canContainRoom) {
			renderer.sprite = Tile_Transparent;
		}
		#endregion

		#region Warnings
		// check for warnings (valid layout)
		GameObject checkIfWarningAlreadyOnScreen = GameObject.Find ("Tile_Warning_" + gameObjectOfTitle.transform.position.x + "/" + gameObjectOfTitle.transform.position.y);
		if (showTile.RoomID != 0) {
			if (checkIfWarningAlreadyOnScreen == null) {
				// create warning game object now		
				checkIfWarningAlreadyOnScreen = new GameObject ();
				// set name of game object to see in Hierarchy
				checkIfWarningAlreadyOnScreen.name = "Tile_Warning_" + gameObjectOfTitle.transform.position.x + "/" + gameObjectOfTitle.transform.position.y;
				// set parent of warning GameObject to the Title game object
				checkIfWarningAlreadyOnScreen.transform.SetParent (gameObjectOfTitle.transform);
				// set X, Y of game object
				checkIfWarningAlreadyOnScreen.transform.position = new Vector2 (gameObjectOfTitle.transform.position.x, gameObjectOfTitle.transform.position.y);
				// add Sprite component
				checkIfWarningAlreadyOnScreen.AddComponent<SpriteRenderer> ();	
				// show above Title = on the Tile_Warning sorting layer  						
				checkIfWarningAlreadyOnScreen.GetComponent<SpriteRenderer> ().sortingLayerName = "Warnings";
			}
			// now it's sure the Tile_Warning_ game object exist, update (or set) it's sprite
			// Show warning if room cannot operate because lake of resources
			bool resourceAvailable = mySub.IsTilePartOfRoomWithResources (showTile);
			if (!resourceAvailable)
				checkIfWarningAlreadyOnScreen.GetComponent<SpriteRenderer> ().sprite = Waring_NoResources;
			// Show warning if room layout is invalid
			bool layoutValid = mySub.IsTilePartOfValidRoomLayout (showTile);
			if (!layoutValid)
				checkIfWarningAlreadyOnScreen.GetComponent<SpriteRenderer> ().sprite = Warning_ToSmall;
			// Show no warnings (remove previous) if all is fine
			if (resourceAvailable && layoutValid)
				checkIfWarningAlreadyOnScreen.GetComponent<SpriteRenderer> ().sprite = Tile_Transparent;
			
		}
		else { // roomID = 0,  don't remove Warning game object but set sprite to transparent (removing gives strange behavior)
			if (checkIfWarningAlreadyOnScreen != null)
				checkIfWarningAlreadyOnScreen.GetComponent<SpriteRenderer> ().sprite = Tile_Transparent;
		}
		#endregion

		#region Wall
		// add wall type
		GameObject checkIfWallIsAlreadyOnScreen = GameObject.Find ("Wall_" + gameObjectOfTitle.transform.position.x + "/" + gameObjectOfTitle.transform.position.y);
		if (showTile.RoomID != 0) {
			// only create GameObject when need to show a wall
			if (checkIfWallIsAlreadyOnScreen == null) { 
				// add Wall now		
				checkIfWallIsAlreadyOnScreen = new GameObject ();
				// set name of game object to see in Hierarchy
				checkIfWallIsAlreadyOnScreen.name = "Wall_" + gameObjectOfTitle.transform.position.x + "/" + gameObjectOfTitle.transform.position.y;
				// set parent of warning GameObject to the Title game object
				checkIfWallIsAlreadyOnScreen.transform.SetParent (gameObjectOfTitle.transform);
				// set X, Y of game object
				checkIfWallIsAlreadyOnScreen.transform.position = new Vector2 (gameObjectOfTitle.transform.position.x, gameObjectOfTitle.transform.position.y);
				// add Sprite Renderer
				checkIfWallIsAlreadyOnScreen.AddComponent<SpriteRenderer> ();

			}
			// now it's sure the Wall game object exist, update (or set) it's sprite
			SpriteRenderer render = checkIfWallIsAlreadyOnScreen.GetComponent<SpriteRenderer> ();
			//set sprite from wall sprite sheet
			//Debug.Log ("For (" + showTile.X + "," + showTile.Y + ") show wall type " + showTile.WallType);
			render.sprite = WallSpriteSheet [showTile.WallType];
			// show above Title = on the Tile_Warning sorting layer  						
			render.sortingLayerName = "Walls";
		}
		else { // roomID = 0,  don't remove Wall game object but set it to wall type 15 = show no walls (removing wall type gives strange behavior)
			if (checkIfWallIsAlreadyOnScreen != null) {
				SpriteRenderer render = checkIfWallIsAlreadyOnScreen.GetComponent<SpriteRenderer> ();
				//set sprite from wall sprite sheet
				//Debug.Log ("Remove all walls for  (" + showTile.X + "," + showTile.Y + ")");
				render.sprite = WallSpriteSheet [15]; // no room = no walls
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

					if (mySub.isCrewType (resourceUnit))
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
		if (GameObject.Find ("ScrollView_CrewButtons") != null) { // only when Crew panel is shown
			GameObject.Find ("Officer place").GetComponent<Text> ().text = mySub.SpacesForOfficers.ToString ();

			GameObject.Find ("Cook place").GetComponent<Text> ().text = mySub.SpacesForCooks.ToString ();

			GameObject.Find ("SonarMan place").GetComponent<Text> ().text = mySub.SpacesForEnlisted.ToString ();
			GameObject.Find ("RadioMan place").GetComponent<Text> ().text = mySub.SpacesForEnlisted.ToString ();
			GameObject.Find ("TorpedoMan place").GetComponent<Text> ().text = mySub.SpacesForEnlisted.ToString ();
			GameObject.Find ("Engineer place").GetComponent<Text> ().text = mySub.SpacesForEnlisted.ToString ();
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
