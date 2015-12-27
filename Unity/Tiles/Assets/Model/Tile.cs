using System;


namespace Submarine.Model {
	public class Tile {
		public int X { get; private set; }

		public int Y { get; private set; }

		int _roomID;

		public int RoomID {
			get{ return _roomID; }
			set {
				_roomID = value;
				if (TileChangedActions != null) // call all the registered callbacks
					TileChangedActions (this);
			}
		}

		public Action<Tile> TileChangedActions { get; set; }
		// functions can registered via this Action to changes of roomID

		public bool canContainRoom { get; set; }
		// used to exclude Tiles that are outside the outline of the sub

		int _wallType;

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
			// use internal field to prevent calling the callback when initiation title (maybee submarine visuale isn't on screen yet)
			_roomID = 0; _wallType = 0; 
			canContainRoom = true;
		}

		public void Reset() {
			RoomID = 0;
			WallType = 0;
			}
	}
}
