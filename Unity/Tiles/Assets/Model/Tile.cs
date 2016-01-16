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

		int _roomID = 0;

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

		int _wallType = 0;

		[UnityEngine.SerializeField]
		public int WallType { 
			get { return _wallType; } 
			set {
				_wallType = value;
				if (TileChangedActions != null) // call all the registered callbacks
					TileChangedActions (this);
			} 
		}
			
		// a Tile can contain Max items
		private List<Piece> _piecesOnTile;

		[UnityEngine.SerializeField]
		// lazy Initialise PiecesOnTile
		public List<Piece> PiecesOnTile { 
			get {
				if (_piecesOnTile == null) {
					_piecesOnTile = new List<Piece> ();
					for (int i = 0; i < Sub.MaxPiecesOnTile; i++) {
						_piecesOnTile.Add (new Piece (PieceType.None, _coord, null));
					}
				}
				return _piecesOnTile;
			} 
			private set {
				_piecesOnTile = value;
			}
		}

	

		#region CONSTRUCTORS

		public Tile () {
		}
		// default constructor needs for initiating List<Tile) when loading sub
		public Tile (int x, int y) {
			_coord = new Point (x, y);
			canContainRoom = true;
		}

		#endregion

		public void Reset () {
			RoomID = 0;
			WallType = 0;

			for (int i = 0; i < Sub.MaxPiecesOnTile; i++) {
				_piecesOnTile.Add (new Piece (PieceType.None, _coord, null));
			}
		}


		#region Pieces

		public void AddItem (Piece itemToAdd) {
			// find empty piece slot
			if (FindPieceOfTypeOnTile (PieceType.None) == null) {
				UnityEngine.Debug.Log ("No more free space on tile (" + X + ", " + Y + ") for piece");
			}
			else {
				// get index of empty slot and fill it with new piece
				int index = PiecesOnTile.FindIndex (p => p.Type == PieceType.None);
				PiecesOnTile [index] = itemToAdd;
			}
		}

		public void RemoveItem (Piece itemToRemove) {
			//			// set piece slot to none and call UI update to transparant is shown
			int index = PiecesOnTile.FindIndex (p => p.carrierID == itemToRemove.carrierID);
			if (index == -1) {
				UnityEngine.Debug.Log ("cannot find piece on tile to remove");
				return;
			}
			PiecesOnTile [index].Reset ();
			// warn UI to redraw
			if (TileChangedActions != null) // call all the registered callbacks
				TileChangedActions (this);
		}

		public Piece FindPieceOfTypeOnTile (PieceType type) {
			return (Piece)PiecesOnTile.Where (p => p.Type == type).FirstOrDefault ();
		}

		#endregion

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
