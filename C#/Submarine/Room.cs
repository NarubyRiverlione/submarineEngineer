using System;
using System.Collections.Generic;
using System.Linq;

namespace Submarine {
    public enum RoomType {
        Empty = 0,
        Engine = 1,
        Generator = 2,
        Battery = 3,
        Bridge=4,
        Gallery=5,
        Mess=6,
        Cabin=7,
        Bunks=8,
        Conn=9,
        Sonar=10,
        Radio=11,
        FuelTank=12,
        BalastTank=13,
        FoodStorage=14,
        EscapeHatch=15,
        TorpedoRoom=16
        };

    //public interface IRoomValidation {
    //    bool isRoomValid(Room checkThisRoom);
    //    }

    abstract public class Room  {
		Sub _inThusSub;	// to get info of the sub (dimensions)
        public RoomType TypeOfRoom { get;  }

        public int Size { get { return spacesInRoom.Count(); } }
        abstract public int MinimimValidSize { get; }
        public List<Space> spacesInRoom { get;protected  set; }

        abstract public double CapacityPerSpace { get; }
        public int Capacity { get { return (int)(Size * CapacityPerSpace); } }
        public int Output { get; protected set; }           // current produced or available cargo
        abstract public string UnitName { get;  }    // unit (liter,..) of output and Capacity

        public bool IsAccessable { get; protected set; } = true;
        abstract public bool IsLayoutValid { get; }

        #region CONSTRUCTOR
		public Room(RoomType ofThisRoomType, Sub sub) {
			TypeOfRoom = ofThisRoomType;
            spacesInRoom = new List<Space>();
			_inThusSub = sub;
            }
        #endregion


        public void AddSpace(Space addSpace) {
            spacesInRoom.Add(addSpace);
            }
        public void RemoveSpace(Space removeSpace) {
            spacesInRoom.Remove(removeSpace);
            }

		public static Room CreateRoomOfType(RoomType type,  Sub inThisSub ) {
            switch(type) {
                case RoomType.FuelTank:
                    return new FuelTank(type,inThisSub);
				case RoomType.Conn:
					return new Conn (type,inThisSub);
                case RoomType.Cabin:
                    return new Cabin(type,inThisSub);
                case RoomType.Bunks:
                    return new Bunks(type,inThisSub);
                case RoomType.Bridge:
                    return new Bridge(type,inThisSub);
                default:
                    throw new NotImplementedException("ERROR: Room type "+ type +" isn't implemented yet.");
                    //return null;
                }
            }

        

        };

    public class FuelTank:Room {
           // public override RoomType TypeOfRoom { get { return RoomType.FuelTank; } }
            public override double CapacityPerSpace{get{ return 1000.0; }}
            public override string UnitName { get { return "liter"; } }
            public override int MinimimValidSize {get { return 4; } }

            public override bool IsLayoutValid { get {
                return Size >= MinimimValidSize ? true : false;
                }
            }

		public FuelTank(RoomType typeOfRoom,Sub sub):base(typeOfRoom,sub) {
                IsAccessable = false;
                }
        }


    public class Cabin : Room {
      //  public override RoomType TypeOfRoom { get { return RoomType.Cabin; } }
        public override double CapacityPerSpace { get { return 2; } }
        public override string UnitName { get { return "officers"; } }
        public override int MinimimValidSize { get { return 4; } }

        public override bool IsLayoutValid
            {
            get
                {
                return Size >= MinimimValidSize ? true : false;
                }
            }

		public Cabin(RoomType typeOfRoom,Sub sub) : base(typeOfRoom,sub) {   }
        }

    public class Bunks : Room {
        // public override RoomType TypeOfRoom { get { return RoomType.Bridge; } }
        public override double CapacityPerSpace { get { return 12.0; } }
        public override string UnitName { get { return "crew"; } }
        public override int MinimimValidSize { get { return 6; } }

        public override bool IsLayoutValid
            {
            get
                {
                return Size >= MinimimValidSize ? true : false;
                }
            }

		public Bunks(RoomType typeOfRoom,Sub sub) : base(typeOfRoom,sub) { }
        }

    public class Conn : Room {
       // public override RoomType TypeOfRoom { get { return RoomType.Bridge; } }
        public override double CapacityPerSpace { get { return 6; } }
        public override string UnitName { get { return "crew"; } }
        public override int MinimimValidSize { get { return 6; } }

		int startValid_X, stopValid_X;
		int valid_Y;

        public override bool IsLayoutValid
            {
            get
                {// check size req.
				bool sizeOk = Size >= MinimimValidSize;

				// should be connected  to the Bride tower
				bool locationValid=false;
				foreach (Space checkSpace in spacesInRoom) {
					if (checkSpace.X > startValid_X && checkSpace.X <= stopValid_X && checkSpace.Y == valid_Y)
						locationValid = true;
				}
				return sizeOk && locationValid; // all req needs true
                }
            }

		public Conn(RoomType typeOfRoom, Sub sub) : base(typeOfRoom,sub) {  
			startValid_X = sub.lengthOfSub /3+2;
			stopValid_X = startValid_X + sub.lenghtOfBridgeTower;
			valid_Y = sub.heightOfBridgeTower + 1;
		}

        }

    public class Bridge : Room {
        // public override RoomType TypeOfRoom { get { return RoomType.Bridge; } }
        public override double CapacityPerSpace { get { return 3.0; } }
        public override string UnitName { get { return "crew"; } }
        public override int MinimimValidSize { get { return 4; } }

        public override bool IsLayoutValid
            {
            get
                {
                return Size >= MinimimValidSize ? true : false;
                }
            }

		public Bridge(RoomType typeOfRoom,Sub sub) : base(typeOfRoom,sub ) { }

        }
    }
