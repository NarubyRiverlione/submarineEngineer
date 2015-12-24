using System;
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
		Mess = 6,
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

	//public interface IRoomValidation {
	//    bool isRoomValid(Room checkThisRoom);
	//    }

	abstract public class Room {
		//Sub _inThusSub;
		// to get info of the sub (dimensions)
		public RoomType TypeOfRoom { get; protected set; }

		public int Size { get { return TilesInRoom.Count (); } }

		abstract public int MinimimValidSize { get; }

		public List<Tile> TilesInRoom { get; protected  set; }

		abstract public double CapacityPerTile { get; }

		public int Capacity { get { return (int)(Size * CapacityPerTile); } }

		public int Output { get; protected set; }
		// current produced or available cargo
		abstract public string UnitName { get; }
		// unit (liter,..) of output and Capacity


		public bool IsAccessable { get; protected set; }

		abstract public bool IsLayoutValid { get; }

		#region CONSTRUCTOR

		public Room (RoomType ofThisRoomType, Sub sub) {
			TypeOfRoom = ofThisRoomType;
			TilesInRoom = new List<Tile> ();
			//_inThusSub = sub;
			IsAccessable = true;
		}

		#endregion


		public void AddTile (Tile addTile) {
			TilesInRoom.Add (addTile);
		}

		public void RemoveTile (Tile removeTile) {
			TilesInRoom.Remove (removeTile);
		}

		public static Room CreateRoomOfType (RoomType type, Sub inThisSub) {
			switch (type) {


				case RoomType.EngineRoom:
					return new EngineRoom (type, inThisSub);
				case RoomType.Generator:
					return new Generator (type, inThisSub);
				case RoomType.Battery:
					return new Battery (type, inThisSub);
				case RoomType.Bridge:
					return new Bridge (type, inThisSub);
				case RoomType.Gallery:
					return new Gallery (type, inThisSub);
				case RoomType.Mess:
					return new Mess (type, inThisSub);
				case RoomType.Cabin:
					return new Cabin (type, inThisSub);
				case RoomType.Bunks:
					return new Bunks (type, inThisSub);
				case RoomType.Conn:
					return new Conn (type, inThisSub);
				case RoomType.Sonar:
					return new Sonar (type, inThisSub);
				case RoomType.RadioRoom:
					return new RadioRoom (type, inThisSub);
				case RoomType.FuelTank:
					return new FuelTank (type, inThisSub);
				case RoomType.BalastTank:
					return new BalastTank (type, inThisSub);
				case RoomType.StorageRoom:
					return new StorageRoom (type, inThisSub);
				case RoomType.EscapeHatch:
					return new EscapeHatch (type, inThisSub);
				case RoomType.TorpedoRoom:
					return new TorpedoRoom (type, inThisSub);
						
				default:
					throw new NotImplementedException ("ERROR: Room type " + type + " isn't implemented yet.");
			//return null;
			}
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

	};


	public class EngineRoom:Room {
		// public override RoomType TypeOfRoom { get { return RoomType.FuelTank; } }
		public override double CapacityPerTile{ get { return 1000.0; } }

		public override string UnitName { get { return "liter"; } }

		public override int MinimimValidSize { get { return 4; } }

		public override bool IsLayoutValid { get { return Size >= MinimimValidSize ? true : false; } }

		public EngineRoom (RoomType typeOfRoom, Sub sub) : base (typeOfRoom, sub) {
			IsAccessable = false;
		}
	}

	public class Generator:Room {
		// public override RoomType TypeOfRoom { get { return RoomType.FuelTank; } }
		public override double CapacityPerTile{ get { return 1000.0; } }

		public override string UnitName { get { return "liter"; } }

		public override int MinimimValidSize { get { return 4; } }

		public override bool IsLayoutValid { get { return Size >= MinimimValidSize ? true : false; } }

		public Generator (RoomType typeOfRoom, Sub sub) : base (typeOfRoom, sub) {
			IsAccessable = false;
		}
	}

	public class Battery:Room {
		// public override RoomType TypeOfRoom { get { return RoomType.FuelTank; } }
		public override double CapacityPerTile{ get { return 1000.0; } }

		public override string UnitName { get { return "liter"; } }

		public override int MinimimValidSize { get { return 4; } }

		public override bool IsLayoutValid { get { return Size >= MinimimValidSize ? true : false; } }

		public Battery (RoomType typeOfRoom, Sub sub) : base (typeOfRoom, sub) {
			IsAccessable = false;
		}
	}

	public class Gallery:Room {
		// public override RoomType TypeOfRoom { get { return RoomType.FuelTank; } }
		public override double CapacityPerTile{ get { return 1000.0; } }

		public override string UnitName { get { return "liter"; } }

		public override int MinimimValidSize { get { return 4; } }

		public override bool IsLayoutValid { get { return Size >= MinimimValidSize ? true : false; } }

		public Gallery (RoomType typeOfRoom, Sub sub) : base (typeOfRoom, sub) {
			IsAccessable = false;
		}
	}

	public class Mess:Room {
		// public override RoomType TypeOfRoom { get { return RoomType.FuelTank; } }
		public override double CapacityPerTile{ get { return 1000.0; } }

		public override string UnitName { get { return "liter"; } }

		public override int MinimimValidSize { get { return 4; } }

		public override bool IsLayoutValid { get { return Size >= MinimimValidSize ? true : false; } }

		public Mess (RoomType typeOfRoom, Sub sub) : base (typeOfRoom, sub) {
			IsAccessable = false;
		}

	}

	public class Sonar:Room {
		// public override RoomType TypeOfRoom { get { return RoomType.FuelTank; } }
		public override double CapacityPerTile{ get { return 1000.0; } }

		public override string UnitName { get { return "liter"; } }

		public override int MinimimValidSize { get { return 4; } }

		public override bool IsLayoutValid { get { return Size >= MinimimValidSize ? true : false; } }

		public Sonar (RoomType typeOfRoom, Sub sub) : base (typeOfRoom, sub) {
			IsAccessable = false;
		}
	}

	public class RadioRoom:Room {
		// public override RoomType TypeOfRoom { get { return RoomType.FuelTank; } }
		public override double CapacityPerTile{ get { return 1000.0; } }

		public override string UnitName { get { return "liter"; } }

		public override int MinimimValidSize { get { return 4; } }

		public override bool IsLayoutValid { get { return Size >= MinimimValidSize ? true : false; } }

		public RadioRoom (RoomType typeOfRoom, Sub sub) : base (typeOfRoom, sub) {
			IsAccessable = false;
		}
	}

	public class StorageRoom:Room {
		// public override RoomType TypeOfRoom { get { return RoomType.FuelTank; } }
		public override double CapacityPerTile{ get { return 1000.0; } }

		public override string UnitName { get { return "liter"; } }

		public override int MinimimValidSize { get { return 4; } }

		public override bool IsLayoutValid { get { return Size >= MinimimValidSize ? true : false; } }

		public StorageRoom (RoomType typeOfRoom, Sub sub) : base (typeOfRoom, sub) {
			IsAccessable = false;
		}
	}

	public class BalastTank:Room {
		// public override RoomType TypeOfRoom { get { return RoomType.FuelTank; } }
		public override double CapacityPerTile{ get { return 1000.0; } }

		public override string UnitName { get { return "liter"; } }

		public override int MinimimValidSize { get { return 4; } }

		public override bool IsLayoutValid { get { return Size >= MinimimValidSize ? true : false; } }

		public BalastTank (RoomType typeOfRoom, Sub sub) : base (typeOfRoom, sub) {
			IsAccessable = false;
		}
	}

	public class EscapeHatch:Room {
		// public override RoomType TypeOfRoom { get { return RoomType.FuelTank; } }
		public override double CapacityPerTile{ get { return 1000.0; } }

		public override string UnitName { get { return "liter"; } }

		public override int MinimimValidSize { get { return 4; } }

		public override bool IsLayoutValid { get { return Size >= MinimimValidSize ? true : false; } }

		public EscapeHatch (RoomType typeOfRoom, Sub sub) : base (typeOfRoom, sub) {
			IsAccessable = false;
		}
	}

	public class FuelTank:Room {
		// public override RoomType TypeOfRoom { get { return RoomType.FuelTank; } }
		public override double CapacityPerTile{ get { return 1000.0; } }

		public override string UnitName { get { return "liter"; } }

		public override int MinimimValidSize { get { return 4; } }

		public override bool IsLayoutValid { get { return Size >= MinimimValidSize ? true : false; } }

		public FuelTank (RoomType typeOfRoom, Sub sub) : base (typeOfRoom, sub) {
			IsAccessable = false;
		}
	}

	public class Cabin : Room {
		//  public override RoomType TypeOfRoom { get { return RoomType.Cabin; } }
		public override double CapacityPerTile { get { return 2; } }

		public override string UnitName { get { return "officers"; } }

		public override int MinimimValidSize { get { return 4; } }

		public override bool IsLayoutValid {
			get {
				return Size >= MinimimValidSize ? true : false;
			}
		}

		public Cabin (RoomType typeOfRoom, Sub sub) : base (typeOfRoom, sub) {
		}
	}

	public class Bunks : Room {
		// public override RoomType TypeOfRoom { get { return RoomType.Bridge; } }
		public override double CapacityPerTile { get { return 12.0; } }

		public override string UnitName { get { return "crew"; } }

		public override int MinimimValidSize { get { return 6; } }

		public override bool IsLayoutValid {
			get {
				return Size >= MinimimValidSize ? true : false;
			}
		}

		public Bunks (RoomType typeOfRoom, Sub sub) : base (typeOfRoom, sub) {
		}
	}

	public class Conn : Room {
		// public override RoomType TypeOfRoom { get { return RoomType.Bridge; } }
		public override double CapacityPerTile { get { return 6; } }

		public override string UnitName { get { return "crew"; } }

		public override int MinimimValidSize { get { return 6; } }

		int startValid_X, stopValid_X;
		int valid_Y;

		public override bool IsLayoutValid {
			get {// check size req.
				bool sizeOk = Size >= MinimimValidSize;

				// should be connected  to the Bride tower
				bool locationValid = false;
				foreach (Tile checkTile in TilesInRoom) {
					if (checkTile.X > startValid_X && checkTile.X <= stopValid_X && checkTile.Y == valid_Y)
						locationValid = true;
				}
				return sizeOk && locationValid; // all req needs true
			}
		}

		public Conn (RoomType typeOfRoom, Sub sub) : base (typeOfRoom, sub) {  
			startValid_X = sub.lengthOfSub / 3 + 2;
			stopValid_X = startValid_X + sub.lenghtOfBridgeTower;
			valid_Y = sub.heightOfBridgeTower + 1;
		}

	}

	public class Bridge : Room {
		// public override RoomType TypeOfRoom { get { return RoomType.Bridge; } }
		public override double CapacityPerTile { get { return 3.0; } }

		public override string UnitName { get { return "crew"; } }

		public override int MinimimValidSize { get { return 4; } }

		public override bool IsLayoutValid {
			get {
				return Size >= MinimimValidSize ? true : false;
			}
		}

		public Bridge (RoomType typeOfRoom, Sub sub) : base (typeOfRoom, sub) {
		}

	}

	public class TorpedoRoom:Room {
		// public override RoomType TypeOfRoom { get { return RoomType.FuelTank; } }
		public override double CapacityPerTile{ get { return 1000.0; } }

		public override string UnitName { get { return "liter"; } }

		public override int MinimimValidSize { get { return 4; } }

		public override bool IsLayoutValid { get { return Size >= MinimimValidSize ? true : false; } }

		public TorpedoRoom (RoomType typeOfRoom, Sub sub) : base (typeOfRoom, sub) {
			IsAccessable = false;
		}
	}
}
