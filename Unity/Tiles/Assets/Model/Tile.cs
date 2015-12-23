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
				if (RoomIDchangedActions != null) // call all the registrated callbacks
					RoomIDchangedActions (this);
			}
		}

		public Action<Tile> RoomIDchangedActions { get; set; }
		// functions can registrated via this Action to changes of roomID

		public bool canContainRoom { get; set; }
		// used to exclude Tiles that are outside the outline of the sub

		public Tile (int x, int y) {
			X = x;
			Y = y;
			_roomID = 0;
			canContainRoom = true;
		}
	}
}
