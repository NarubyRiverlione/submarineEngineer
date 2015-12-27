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
		//	Mess = 6,
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
       
		public RoomType TypeOfRoom { get; protected set; }

		public int Size{ get { return TilesInRoom.Count (); } }

		public int MinimimValidSize { get; protected set; }

		public string ValidationText { get; protected set; }

		public List<Tile> TilesInRoom { get; protected set; }

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

		public Room (RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, string unitOfCap) {    // sub: to get info of the sub (dimensions)
			TypeOfRoom = ofThisRoomType;
			TilesInRoom = new List<Tile> ();
			IsAccessable = true;
			MinimimValidSize = minSize;
			CapacityPerTile = capPerTile;
			UnitName = unitOfCap;
			// don't stop scentese with a '.', maybee a concrete class will add aditional requirements
			ValidationText = "The " + ofThisRoomType + " needs to be at least " + MinimimValidSize + " spaces";
		}

		#endregion

		public static Room CreateRoomOfType (RoomType ofThisRoomType, Sub inThisSub) {
			// let factory create the correct concrete class
			return RoomFactory.CreateRoomOfType (ofThisRoomType, inThisSub);
		}

		public void AddTile (Tile addTile) {
			TilesInRoom.Add (addTile);
		}

		public void RemoveTile (Tile removeTile) {
			TilesInRoom.Remove (removeTile);
            // reset Tile (roomId, waalType,...)
            removeTile.Reset();
            }

		public void WarnTilesInRoomIfLayoutChanged (bool oldRoomLayoutValid) {
			if (oldRoomLayoutValid != IsLayoutValid) {
				Debug.WriteLine ("Validation of room layout has changed, warn title of room");
				foreach (Tile warnTile in TilesInRoom) {
					if (warnTile.TileChangedActions != null)
						warnTile.TileChangedActions (warnTile);
				}
			}
		}


	}

    ;
}