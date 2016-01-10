using System;
using System.Collections.Generic;
using System.Linq;


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

		static public int MaxItems = 2;
		// a Tile can contain Max items
		public List<Piece> PiecesOnTile { get; private set; }

	

		public Tile () {
		}
		// default constructor needs for initiating List<Tile) when loading sub
		public Tile (int x, int y) {
			_coord = new Point (x, y);
			Reset ();
			canContainRoom = true;
			PiecesOnTile = new List<Piece> ();
		}

		public void Reset () {
			RoomID = 0;
			WallType = 0;

		}

		public void AddItem (Piece itemToAdd) {
			// a Tile can contain MaxItems and only 1 of the same type (no 2 Pipes in 1 tile)
			if (PiecesOnTile.Count < MaxItems && FindPieceOfTypeOnTile (itemToAdd.Type) == null) {
				PiecesOnTile.Add (itemToAdd);
				// don't redraw Tile now, do it via the piece.carrierID so its sure all needed info is in the piece
//				if (TileChangedActions != null)
//					TileChangedActions (this);
			}
		}

		public void RemoveItem (Piece itemToRemove) {
			PiecesOnTile.Remove (itemToRemove);
			if (TileChangedActions != null)
				TileChangedActions (this);
		}

		public Piece FindPieceOfTypeOnTile (PieceType type) {
			return PiecesOnTile.Where (p => p.Type == type).FirstOrDefault ();
		}

	

		public bool IsWalkable () {
			bool walkable = true;
			// tiles that hold a piece isn't walkable any more
			if (PiecesOnTile.Count > 0)
				walkable = false;

			//TODO: only lowest Tiles of a room are walkable

			return walkable;
		}
	}
}
