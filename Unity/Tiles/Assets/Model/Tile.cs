using System;


namespace Submarine.Model {
	public class Tile {
		public int X { get; private set; }

		public int Y { get; private set; }

		int _roomID;

		[UnityEngine.SerializeField]
		public int RoomID {
			get{ return _roomID; }
			set {
//				if (value == 0 && _roomID != 0)
//					UnityEngine.Debug.Log ("RoomID reset for (" + X + "," + Y + ")");
				
				_roomID = value;

				if (TileChangedActions != null) // call all the registered callbacks
					TileChangedActions (this);
				
			}
		}

		Action<Tile> _TileChangedActions;

		public Action<Tile> TileChangedActions {
			get { return _TileChangedActions; } 
			set { 
				_TileChangedActions = value; 

			}
		}
		// functions can registered via this Action to changes of roomID

		public bool canContainRoom { get; set; }
		// used to exclude Tiles that are outside the outline of the sub

		int _wallType;

		[UnityEngine.SerializeField]
		public int WallType { 
			get { return _wallType; } 
			set {
				_wallType = value;
				if (TileChangedActions != null) // call all the registered callbacks
					TileChangedActions (this);
			} 
		}


		public Tile (int x, int y) {
			X = x;
			Y = y;
			Reset ();
			canContainRoom = true;
		}

		public void Reset () {
			RoomID = 0;
			WallType = 0;
		}
	}
}
