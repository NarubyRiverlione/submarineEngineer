using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Submarine.Model {
	public enum RoomType {
		Empty = 0,
		EngineRoom = 1,
		Generator = 2,
		Battery = 3,
		Bridge = 4,
		Gallery = 5,
		//	PumpRoom = 6,
		Cabin = 7,
		Bunks = 8,
		Conn = 9,
		Sonar = 10,
		RadioRoom = 11,
		FuelTank = 12,
		BalastTank = 13,
		StorageRoom = 14,
		EscapeHatch = 15,
		TorpedoRoom = 16}

	;


	abstract public class Room {
       
		public Sub inSub { get; protected set; }
		// needs reference to sub for layout validation

		public RoomType TypeOfRoom { get; protected set; }

		public int Size{ get { return coordinatesOfTilesInRoom.Count (); } }

		public int MinimimValidSize { get; protected set; }

		public string ValidationText { get; protected set; }

		//public List<Tile> TilesInRoom { get; protected set; }
		public List<Point> coordinatesOfTilesInRoom;

		public double CapacityPerTile { get; protected set; }

		public int capPerTileacity {
			get{ return (int)(Size * CapacityPerTile); }
		}

		public int Output { get; protected set; }

		// current produced or available cargo
		public string UnitName { get; protected set; }

		// unit (liter,..) of output and Capacity
		public bool IsAccessable { get; protected set; }

		abstract public bool IsLayoutValid { get; }

		#region CONSTRUCTOR

		public Room (RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, string unitOfCap) {    // sub: to get info of the sub (dimensions) for extra layout validation of some room types
			TypeOfRoom = ofThisRoomType;
			coordinatesOfTilesInRoom = new List<Point> ();
			IsAccessable = true;
			MinimimValidSize = minSize;
			CapacityPerTile = capPerTile;
			UnitName = unitOfCap;
			inSub = sub;
			// don't stop scentese with a '.', maybee a concrete class will add aditional requirements
			ValidationText = "The " + ofThisRoomType + " needs to be at least " + MinimimValidSize + " spaces";
		}

		#endregion

		public static Room CreateRoomOfType (RoomType ofThisRoomType, Sub inThisSub) {
			// let factory create the correct concrete class
			return RoomFactory.CreateRoomOfType (ofThisRoomType, inThisSub);
		}

		public void AddTile (Tile addTile) {
			//TilesInRoom.Add (addTile);
			Point coord = new Point (addTile.X, addTile.Y);
			coordinatesOfTilesInRoom.Add (coord);
		}

		public void RemoveTile (Tile removeTile) {
			//TilesInRoom.Remove (removeTile);
			Point coord = new Point (removeTile.X, removeTile.Y);
			coordinatesOfTilesInRoom.Remove (coord);
			// reset Tile (roomId, waalType,...)
			// removeTile.Reset ();
		}

		public void WarnTilesInRoomThatLayoutChanged (bool oldRoomLayoutValid) {
			if (oldRoomLayoutValid != IsLayoutValid) {
				//Debug.WriteLine ("Validation of room layout has changed, warn title of room");
				foreach (Tile warnTile in GetTilesInAroom()) {
					if (warnTile.TileChangedActions != null)
						warnTile.TileChangedActions (warnTile);
				}				
			}
		}

		public List<Tile> GetTilesInAroom () {
			List<Tile> tiles = new List<Tile> ();
			foreach (Point coord in coordinatesOfTilesInRoom) {
				Tile tile = inSub.GetTileAt (coord.x, coord.y);
				tiles.Add (tile);
			}
			return tiles;
		}


	}
}