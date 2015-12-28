using Submarine.Model;
using UnityEngine;
using UnityEngine.UI;

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
	public Sprite Tile_PumpRoom;
	public Sprite Tile_StorageRoom;
	public Sprite Tile_EscapeHatch;
	public Sprite Tile_TorpedoRoom;

	// Warning Sprites
	public Sprite Tile_Warning;

	// Wall spritesheet (private, loaded in Start)
	Sprite[] WallSpriteSheet;

	// UI Top text
	public Text UI_Resources_Text;

	// Use this for initialization
	void Start () {
		instance = this;
		mySub = new Sub ();
		//Debug.Log ("Sub created with length:" + mySub.lengthOfSub + " & height " + mySub.heightOfSub);

		// loading Wall sprites from sheet
		WallSpriteSheet = Resources.LoadAll<Sprite> ("Walls");

		CreateAllTileGameObjects ();
	}
	
	// Update is called once per frame
	void Update () {
		UI_Resources_Text.text = mySub.GetAllOutputs ();
	}

	public void CreateAllTileGameObjects () {
		// Create GameObject for each Tile
		for (int x = 0; x < mySub.lengthOfSub; x++) {
			for (int y = 0; y < mySub.heightOfSub; y++) {
				Tile newTile = mySub.GetTileAt (x, y);
				GameObject newTileSprite = new GameObject ();
				// set name of game object to see in Hierarchy
				newTileSprite.name = "Tile_" + x + "/" + y;
				// set WorldController as parten
				newTileSprite.transform.SetParent (this.transform);
				// set X, Y of game object
				newTileSprite.transform.position = new Vector2 (newTile.X, newTile.Y);
				// add Sprite Renderer component
				newTileSprite.AddComponent<SpriteRenderer> ();
				// set sprite
				UpdateTileSprite (newTile, newTileSprite);
				// when the roomID of the title changes, update the sprite
				newTile.TileChangedActions += (tile => {
					UpdateTileSprite (tile, newTileSprite);
				});
			}
		}
	}

	public void RemoveAllTileGameObjects () {
		for (int x = 0; x < mySub.lengthOfSub; x++) {
			for (int y = 0; y < mySub.heightOfSub; y++) {
				string name = "Tile_" + x + "/" + y;
				GameObject tile_gameObject = GameObject.Find (name);
				Destroy (tile_gameObject); // destroys also all child game objects
			}
		}
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
			case RoomType.PumpRoom:
				renderer.sprite = Tile_PumpRoom;
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
			// now it's sure the Wall game object exist, update (or set) it's sprite
			if (!layoutValid)
				checkIfWarningAlreadyOnScreen.GetComponent<SpriteRenderer> ().sprite = Tile_Warning;
			else
				checkIfWarningAlreadyOnScreen.GetComponent<SpriteRenderer> ().sprite = Tile_Transparent;
			
		}
		else { // roomID = 0,  don't remove Warning game object but set sprite to transparant (removeing gives strange behaviour)
			if (checkIfWarningAlreadyOnScreen != null)
				checkIfWarningAlreadyOnScreen.GetComponent<SpriteRenderer> ().sprite = Tile_Transparent;
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
			Debug.Log ("For (" + showTile.X + "," + showTile.Y + ") show wall type " + showTile.WallType);
			render.sprite = WallSpriteSheet [showTile.WallType];
			// show above Title = on the Tile_Warning sorting layer  						
			render.sortingLayerName = "Walls";
		}
		else { // roomID = 0,  don't remove Wall game object but set it to wall type 15 = show no walls (removeing wall type gives strange behaviour)
			if (checkIfWallIsAlreadyOnScreen != null) {
				//Destroy (checkIfWallIsAlreadyOnScreen);
				SpriteRenderer render = checkIfWallIsAlreadyOnScreen.GetComponent<SpriteRenderer> ();
				//set sprite from wall sprite sheet
				Debug.Log ("Remove all walls for  (" + showTile.X + "," + showTile.Y + ")");
				render.sprite = WallSpriteSheet [15]; // no room = no walls
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
