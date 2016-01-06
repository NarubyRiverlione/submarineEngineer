using System;
using System.Collections.Generic;


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
		public List<Piece> Pieces { get; private set; }

		static public int MaxItems = 2;

		public Tile (int x, int y) {
			_coord = new Point (x, y);
			Reset ();
			canContainRoom = true;
			Pieces = new List<Piece> (); 	

		}

		public void Reset () {
			RoomID = 0;
			WallType = 0;
		}

		public void AddItem (Piece itemToAdd) {
			if (Pieces.Count < MaxItems) // a Tile can contain MaxItems
				Pieces.Add (itemToAdd);
			if (TileChangedActions != null)
				TileChangedActions (this);
		}

		public void RemoveItem (Piece itemToRemove) {
			Pieces.Remove (itemToRemove);
			if (TileChangedActions != null)
				TileChangedActions (this);
		}
	}
}
