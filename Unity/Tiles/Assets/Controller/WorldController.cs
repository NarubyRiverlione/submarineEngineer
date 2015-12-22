using UnityEngine;
using System.Collections;
using Submarine.Model;

namespace Submarine.Controller {
	public class WorldController : MonoBehaviour {

		Sub mySub;
		// Sprites
		public Sprite Space_Empty;
		public Sprite Space_Water;
		public Sprite Space_Bridge;
		public Sprite Space_Unknown;

		// Use this for initialization
		void Start () {
			mySub = new Sub ();
			Debug.Log ("Sub created with lenght:" + mySub.lengthOfSub + " & height " + mySub.heightOfSub);



			// Create GameObject for each Space
			for (int x = 0; x < mySub.lengthOfSub; x++) {
				for (int y = 0; y < mySub.heightOfSub; y++) {
					Tile showThisSpace = mySub.GetSpaceAt (x, y);
					GameObject newSpace = new GameObject ();
					newSpace.name = "Space_" + x + "/" + y;													// set name of game object to see in Hierarchy
					newSpace.transform.position = new Vector2 (showThisSpace.X, showThisSpace.Y);				// set X, Y of game object
                    
					SpriteRenderer newSpace_Renderer = newSpace.AddComponent<SpriteRenderer> ();			// add Sprite Renderer component
					switch (mySub.GetRoomTypeOfSpace (showThisSpace)) {
						case RoomType.Empty:
							newSpace_Renderer.sprite = Space_Empty;
							break;
						case RoomType.Bridge:
							//newSpace_Renderer.sprite = Space_Bridge;
							break;
						default:
						//	newSpace_Renderer.sprite = Space_Unknown;
							break;
					}

					if (!showThisSpace.canContainRoom) {// cannot be build on = outside sub = show water
						newSpace_Renderer.sprite = Space_Water;
					}
                    
				}
			}
		}
	
		// Update is called once per frame
		void Update () {
	
		}
	}
}