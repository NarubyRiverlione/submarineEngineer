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
		
		Cabin = 7,
		Bunks = 8,
		Conn = 9,
		Sonar = 10,
		RadioRoom = 11,
		FuelTank = 12,
		PumpRoom = 13,
		StorageRoom = 14,
		EscapeHatch = 15,
		TorpedoRoom = 16}

	;

	public enum Units {
		pks,
		MWs,
		AH,
		liters_fuel,
		liters_pump,
		food,
		tins,
		Engineers,
		Cook,
		Crew,
		Officer,
		Lookouts,
		Sonarman,
		Radioman,
		TorpedoMan,
		Torpedoes,
		None}

	;

	abstract public class Room {
       
		public Sub inSub { get; protected set; }
		// needs reference to sub for layout validation
		public RoomType TypeOfRoom { get; protected set; }
		//public List<Tile> TilesInRoom { get; protected set; }
		public List<Point> coordinatesOfTilesInRoom;

		public int Size{ get { return coordinatesOfTilesInRoom.Count (); } }

		public int MinimimValidSize { get; protected set; }

		public string ValidationText { get; protected set; }

		abstract public bool IsLayoutValid { get; }


		public double CapacityPerTile { get; protected set; }

		public virtual int RoomCapacity {
			get{ return (int)(Size * CapacityPerTile); }
		}
		// current produced or available cargo
		public Units UnitOfCapacity { get; protected set; }

	

		//		public int ReqCrew { get; private set; }
		//
		//		public int CurrentCrew { get; private set; }
		//
		//		public Units CrewUnit { get; private set; }
		//
		//		public int RoomCrewPreformance {
		//			get{ return (int)(CurrentCrew / ReqCrew * 100); }
		//		}

		public int ReqResource { get; private set; }

		public int CurrentResource { get; set; }

		public Units ResourceUnit { get; private set; }


		public int Output { 
			get { // only output is layout is valid
				if (IsLayoutValid && CurrentResource >= ReqResource)
					return RoomCapacity;
				else
					return 0;
			}
		}

		//		public string OutputText {  // only show output text is there is output (Bridge, Conn, Sonar, Radio, have no output atm)
		//			get{ return UnitOfCapacity != Units.None ? "(" + TypeOfRoom + ") " + Output + " " + UnitOfCapacity : ""; }
		//		}

	

		// unit (liter,..) of output and Capacity
		public bool IsAccessable { get; protected set; }



		#region CONSTRUCTOR

		public Room (RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, Units unitOfCap, Units resource, int reqRes) {    // sub: to get info of the sub (dimensions) for extra layout validation of some room types
			TypeOfRoom = ofThisRoomType;
			coordinatesOfTilesInRoom = new List<Point> ();
			IsAccessable = true;
			MinimimValidSize = minSize;
			CapacityPerTile = capPerTile;
			UnitOfCapacity = unitOfCap;
			inSub = sub;
			ResourceUnit = resource;
			reqRes = ReqResource;

			// don't stop scentese with a '.', maybee a concrete class will add aditional requirements
			ValidationText = "The " + ofThisRoomType + " needs to be at least " + MinimimValidSize + " spaces";
		}

		#endregion

		public static Room CreateRoomOfType (RoomType ofThisRoomType, Sub inThisSub) {
			// let factory create the correct concrete class
			return RoomFactory.CreateRoomOfType (ofThisRoomType);
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
				foreach (Point coord in coordinatesOfTilesInRoom) {
					Tile warnTile = inSub.GetTileAt (coord.x, coord.y);
					if (warnTile.TileChangedActions != null)
						warnTile.TileChangedActions (warnTile);
				}
			}				
		}



	}
}