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
	public Sprite WallSpriteSheet;

	// Use this for initialization
	void Start () {
		instance = this;
		mySub = new Sub ();
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

	void UpdateTileSprite (Tile showTile, GameObject spriteOfTile) {
		SpriteRenderer renderer = spriteOfTile.GetComponent<SpriteRenderer> ();
		switch (mySub.GetRoomTypeOfTile (showTile)) {
			case RoomType.Empty:
				renderer.sprite = Tile_Empty;break;
			case RoomType.Bridge:
				renderer.sprite = Tile_Bridge; break;
			case RoomType.EngineRoom:
				renderer.sprite = Tile_EngineRoom;break;
			case RoomType.Generator:
				renderer.sprite = Tile_Generator;break;
			case RoomType.Battery:
				renderer.sprite = Tile_Battery;break;
			case RoomType.Gallery:
				renderer.sprite = Tile_Gallery;break;
			case RoomType.Mess:
				renderer.sprite = Tile_Mess;break;
			case RoomType.Cabin:
				renderer.sprite = Tile_Cabin;break;
			case RoomType.Bunks:
				renderer.sprite = Tile_Bunks;break;
			case RoomType.Conn:
				renderer.sprite = Tile_Conn;break;
			case RoomType.Sonar:
				renderer.sprite = Tile_Sonar;break;
			case RoomType.RadioRoom:
				renderer.sprite = Tile_RadioRoom;break;
			case RoomType.FuelTank:
				renderer.sprite = Tile_FuelTank;break;
			case RoomType.BalastTank:
				renderer.sprite = Tile_BalastTank;break;
			case RoomType.StorageRoom:
				renderer.sprite = Tile_StorageRoom;break;
			case RoomType.EscapeHatch:
				renderer.sprite = Tile_EscapeHatch;break;
			case RoomType.TorpedoRoom:
				renderer.sprite = Tile_TorpedoRoom;break;
			default:
				renderer.sprite = Tile_Unknown;break;
		}
		// cannot be build on = outside sub = show transparent
		if (showTile != null && !showTile.canContainRoom) {
			renderer.sprite = Tile_Transparent;
		}

		// check for warnings (valid layout)
		if (showTile.RoomID != 0) {
			GameObject checkIfWarningAlreadyOnScreen = GameObject.Find ("Tile_Warning_" + spriteOfTile.transform.position.x + "/" + spriteOfTile.transform.position.y);
			var layoutValid = mySub.IsTilePartOfValidRoomLayout (showTile);
			if (!layoutValid && checkIfWarningAlreadyOnScreen == null) {
				// add warning now		
				GameObject newTileWarningSprite = new GameObject ();
				// set name of game object to see in Hierarchy
				newTileWarningSprite.name = "Tile_Warning_" + spriteOfTile.transform.position.x + "/" + spriteOfTile.transform.position.y;
				// set parent of warning GameObject
				newTileWarningSprite.transform.SetParent (this.transform);
				// set X, Y of game object
				newTileWarningSprite.transform.position = new Vector2 (spriteOfTile.transform.position.x, spriteOfTile.transform.position.y);
				// show above Title = on the Tile_Warning sorting layer  						
				newTileWarningSprite.layer = SortingLayer.GetLayerValueFromName ("Tile_Warning");
				// set Sprite
				SpriteRenderer render = newTileWarningSprite.AddComponent<SpriteRenderer> ();	
				render.sprite = Tile_Warning;
			}
			if (layoutValid && checkIfWarningAlreadyOnScreen != null) {
				// remove warning
				Destroy (checkIfWarningAlreadyOnScreen);
			}
		}

		// add wall type
		if (showTile.RoomID != 0 ) {
			GameObject checkIfWallIsAlreadyOnScreen = GameObject.Find("Wall_" + spriteOfTile.transform.position.x + "/" + spriteOfTile.transform.position.y);

			if (checkIfWallIsAlreadyOnScreen == null) {
				// add Wall now		
				//GameObject newWall = new GameObject();
				// set name of game object to see in Hierarchy
				checkIfWallIsAlreadyOnScreen.name = "Wall" + spriteOfTile.transform.position.x + "/" + spriteOfTile.transform.position.y;
				// set parent of warning GameObject
				checkIfWallIsAlreadyOnScreen.transform.SetParent(this.transform);
				// set X, Y of game object
				checkIfWallIsAlreadyOnScreen.transform.position = new Vector2(spriteOfTile.transform.position.x, spriteOfTile.transform.position.y);
				// show above Title = on the Tile_Warning sorting layer  						
				checkIfWallIsAlreadyOnScreen.layer = SortingLayer.GetLayerValueFromName("Walls");
				// add Sprite
				checkIfWallIsAlreadyOnScreen.AddComponent<SpriteRenderer>();
				}
			// now it's sure the Wall gameobject exist, update (or set) it's sprite
			SpriteRenderer render = checkIfWallIsAlreadyOnScreen.GetComponent<SpriteRenderer>();
			render.sprite = WallSpriteSheet;
			}

	}

	// get Tile at x,y in World
	public Tile GetTileAtWorldCoordinates (Vector3 coord) {
		int x = Mathf.FloorToInt (coord.x);
		int y = Mathf.FloorToInt (coord.y);

		return mySub != null ? mySub.GetTileAt (x, y) : null;
	}

}
