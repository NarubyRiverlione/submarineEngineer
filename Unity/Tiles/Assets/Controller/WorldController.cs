using Submarine.Model;
using UnityEngine;


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
	public Sprite Tile_Mess;
	public Sprite Tile_Cabin;
	public Sprite Tile_Bunks;
	public Sprite Tile_Conn;
	public Sprite Tile_Sonar;
	public Sprite Tile_RadioRoom;
	public Sprite Tile_FuelTank;
	public Sprite Tile_BalastTank;
	public Sprite Tile_StorageRoom;
	public Sprite Tile_EscapeHatch;
	public Sprite Tile_TorpedoRoom;


	// Warning Sprites
	public Sprite Tile_Warning;

	// Wall sprite (sheet)
	Sprite[] WallSpriteSheet;

	// Use this for initialization
	void Start () {
		instance = this;
		mySub = new Sub ();
		// loading Wall sprites from sheet
		WallSpriteSheet = Resources.LoadAll<Sprite> ("Walls");

		Debug.Log ("Sub created with length:" + mySub.lengthOfSub + " & height " + mySub.heightOfSub);

		// Create GameObject for each Tile
		for (int x = 0; x < mySub.lengthOfSub; x++) {
			for (int y = 0; y < mySub.heightOfSub; y++) {
				Tile newTile = mySub.GetTileAt (x, y);
				GameObject newTileSprite = new GameObject ();
				newTileSprite.name = "Tile_" + x + "/" + y;                                                 // set name of game object to see in Hierarchy
				newTileSprite.transform.SetParent (this.transform);
				newTileSprite.transform.position = new Vector2 (newTile.X, newTile.Y);						// set X, Y of game object
				   
				newTileSprite.AddComponent<SpriteRenderer> ();												// add Sprite Renderer component
				UpdateTileSprite (newTile, newTileSprite);													// set sprite

				newTile.TileChangedActions += ((tile) => { // when the roomID of the title changes, update the sprite
					UpdateTileSprite (tile, newTileSprite);
				});
					

			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void UpdateTileSprite (Tile showTile, GameObject gameObjectOfTitle) {
		SpriteRenderer renderer = gameObjectOfTitle.GetComponent<SpriteRenderer> ();
		switch (mySub.GetRoomTypeOfTile (showTile)) {
			case RoomType.Empty:
				renderer.sprite = Tile_Empty;
				break;
			case RoomType.Bridge:
				renderer.sprite = Tile_Bridge;
				break;
			case RoomType.EngineRoom:
				renderer.sprite = Tile_EngineRoom;
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
			case RoomType.Conn:
				renderer.sprite = Tile_Conn;
				break;
			case RoomType.Sonar:
				renderer.sprite = Tile_Sonar;
				break;
			case RoomType.RadioRoom:
				renderer.sprite = Tile_RadioRoom;
				break;
			case RoomType.FuelTank:
				renderer.sprite = Tile_FuelTank;
				break;
			case RoomType.BalastTank:
				renderer.sprite = Tile_BalastTank;
				break;
			case RoomType.StorageRoom:
				renderer.sprite = Tile_StorageRoom;
				break;
			case RoomType.EscapeHatch:
				renderer.sprite = Tile_EscapeHatch;
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

		// check for warnings (valid layout)
		GameObject checkIfWarningAlreadyOnScreen = GameObject.Find ("Tile_Warning_" + gameObjectOfTitle.transform.position.x + "/" + gameObjectOfTitle.transform.position.y);
		if (showTile.RoomID != 0) {
			var layoutValid = mySub.IsTilePartOfValidRoomLayout (showTile);
			if (!layoutValid && checkIfWarningAlreadyOnScreen == null) {
				// add warning now		
				GameObject newTileWarningSprite = new GameObject ();
				// set name of game object to see in Hierarchy
				newTileWarningSprite.name = "Tile_Warning_" + gameObjectOfTitle.transform.position.x + "/" + gameObjectOfTitle.transform.position.y;
				// set parent of warning GameObject to the Title gameobject
				newTileWarningSprite.transform.SetParent (gameObjectOfTitle.transform);
				// set X, Y of game object
				newTileWarningSprite.transform.position = new Vector2 (gameObjectOfTitle.transform.position.x, gameObjectOfTitle.transform.position.y);
				// set Sprite
				SpriteRenderer render = newTileWarningSprite.AddComponent<SpriteRenderer> ();	
				render.sprite = Tile_Warning;
				// show above Title = on the Tile_Warning sorting layer  						
				render.sortingLayerName = "Tile_Warning";

			}
			if (layoutValid && checkIfWarningAlreadyOnScreen != null) {
				// remove warning
				Destroy (checkIfWarningAlreadyOnScreen);
			}
		}
		else { // roomID = 0, check if warning GameObject still exists, remove it now because room doesn't exist any more on this tile
			if (checkIfWarningAlreadyOnScreen != null) {
				Destroy (checkIfWarningAlreadyOnScreen);
			}
		}
		// add wall type
		GameObject checkIfWallIsAlreadyOnScreen = GameObject.Find ("Wall_" + gameObjectOfTitle.transform.position.x + "/" + gameObjectOfTitle.transform.position.y);

		if (showTile.RoomID != 0) {
			// only create GameObject when need to show a wall
			if (checkIfWallIsAlreadyOnScreen == null) { 
				// add Wall now		
				checkIfWallIsAlreadyOnScreen = new GameObject ();
				// set name of game object to see in Hierarchy
				checkIfWallIsAlreadyOnScreen.name = "Wall_" + gameObjectOfTitle.transform.position.x + "/" + gameObjectOfTitle.transform.position.y;
				// set parent of warning GameObject to the Title gameobject
				checkIfWallIsAlreadyOnScreen.transform.SetParent (gameObjectOfTitle.transform);
				// set X, Y of game object
				checkIfWallIsAlreadyOnScreen.transform.position = new Vector2 (gameObjectOfTitle.transform.position.x, gameObjectOfTitle.transform.position.y);
				// add Sprite Renderer
				checkIfWallIsAlreadyOnScreen.AddComponent<SpriteRenderer> ();

			}
			// now it's sure the Wall gameobject exist, update (or set) it's sprite
			SpriteRenderer render = checkIfWallIsAlreadyOnScreen.GetComponent<SpriteRenderer> ();
			//set sprite from wall sprite sheet
			//Debug.Log ("Try showing wall type " + showTile.WallType);
			render.sprite = WallSpriteSheet [showTile.WallType];
			// show above Title = on the Tile_Warning sorting layer  						
			render.sortingLayerName = "Walls";
		}
		else { // roomID = 0, check if Wall GameObject still exists, remove it now because room doesn't exist any more on this tile
			if (checkIfWallIsAlreadyOnScreen != null) {
				Destroy (checkIfWallIsAlreadyOnScreen);
			}
			
		}

	}

	// get Tile at x,y in World
	public Tile GetTileAtWorldCoordinates (Vector3 coord) {
		int x = Mathf.FloorToInt (coord.x);
		int y = Mathf.FloorToInt (coord.y);

		return mySub != null ? mySub.GetTileAt (x, y) : null;
	}

}
