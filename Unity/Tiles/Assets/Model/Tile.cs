using System;


namespace Submarine.Model {
	public class Tile {
		[UnityEngine.SerializeField]
		private Point _coord;

		public int X { get { return _coord.x; } }

		public int Y { get { return _coord.y; } }

		// functions can registered via this Action to be informed when tile is changed
		public Action<Tile> TileChangedActions { get; set; }

		int _roomID;

		[UnityEngine.SerializeField]
		public int RoomID {
			get{ return _roomID; }
			set {
				_roomID = value;
				if (TileChangedActions != null) // call all the registered callbacks
					TileChangedActions (this);
			}
		}

		// used to exclude Tiles that are outside the outline of the sub
		public bool canContainRoom { get; set; }

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

		// a Tile can contain 2 items
		public Piece[] pieces { get; private set; }

		static public int MaxItems = 2;

		public Tile (int x, int y) {
			_coord = new Point (x, y);
			Reset ();
			canContainRoom = true;
			pieces = new Piece [MaxItems]; 	// a Tile can contain 2 items
		}

		public void Reset () {
			RoomID = 0;
			WallType = 0;
		}

		public void AddItem (Piece itemToAdd) {
			if (pieces.Length < MaxItems)
				pieces [pieces.Length] = itemToAdd;
		}

		public void RemoveItem (Piece itemToRemove) {
			int index = Array.IndexOf (pieces, itemToRemove);
			if (index != -1)
				Array.Clear (pieces, index, 1);
			else
				throw new Exception ("No " + itemToRemove + " on tile (" + X + "," + Y + ")");
		}
	}
}
