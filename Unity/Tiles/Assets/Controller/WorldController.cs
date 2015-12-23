using UnityEngine;
using System.Collections;
using Submarine.Model;

namespace Submarine.Controller {
	public class WorldController : MonoBehaviour {

		// instance to become a singleton
		public static WorldController instance { get; private set; }

		// Model
		public Sub mySub { get; private set; }

		// Sprites
		public Sprite Tile_Empty;
		public Sprite Tile_Transparent;
		public Sprite Tile_Bridge;
		public Sprite Tile_Unknown;

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

					newTile.RoomIDchangedActions += ((tile) => { // when the roomID of the title changes, update the sprite
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
					renderer.sprite = Tile_Empty;
					break;
				case RoomType.Bridge:
					renderer.sprite = Tile_Bridge;
					break;
				default:
					renderer.sprite = Tile_Unknown;
					break;
			}

			if (showTile != null && !showTile.canContainRoom) {// cannot be build on = outside sub = show transparant
				renderer.sprite = Tile_Transparent;
			}

		}

		// get Tile at x,y in World
		public Tile GetTileAtWorldCoordinates (Vector3 coord) {
			int x = Mathf.FloorToInt (coord.x);
			int y = Mathf.FloorToInt (coord.y);

			return mySub.GetTileAt (x, y);
		}

	}
}