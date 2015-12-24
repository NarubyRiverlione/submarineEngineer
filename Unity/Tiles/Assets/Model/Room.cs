using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Submarine.Model
{
    public enum RoomType
    {
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
        TorpedoRoom = 16
    }

    ;
    abstract public class Room
    {
       
        public RoomType TypeOfRoom
        {
            get;
            protected set;
        }

        public int Size
        {
            get
            {
                return TilesInRoom.Count();
            }
        }

        abstract public int MinimimValidSize
        {
            get;
        }

        public List<Tile> TilesInRoom
        {
            get;
            protected set;
        }

        abstract public double CapacityPerTile
        {
            get;
        }

        public int Capacity
        {
            get
            {
                return (int)(Size * CapacityPerTile);
            }
        }

        public int Output
        {
            get;
            protected set;
        }

        // current produced or available cargo
        abstract public string UnitName
        {
            get;
        }

        // unit (liter,..) of output and Capacity
        public bool IsAccessable
        {
            get;
            protected set;
        }

        abstract public bool IsLayoutValid
        {
            get;
        }

#region CONSTRUCTOR
        public Room(RoomType ofThisRoomType, Sub sub)    // sub: to get info of the sub (dimensions)
        {
            TypeOfRoom = ofThisRoomType;
            TilesInRoom = new List<Tile>();
            IsAccessable = true;
        }

#endregion
        public static Room CreateRoomOfType(RoomType ofThisRoomType, Sub inThisSub)
        {
            // let factory create the correct concrete class
            return RoomFactory.CreateRoomOfType(ofThisRoomType, inThisSub);
        }

        public void AddTile(Tile addTile)
        {
            TilesInRoom.Add(addTile);
        }

        public void RemoveTile(Tile removeTile)
        {
            TilesInRoom.Remove(removeTile);
        }

        public void WarnTilesInRoomIfLayoutChanged(bool oldRoomLayoutValid)
        {
            if (oldRoomLayoutValid != IsLayoutValid)
            {
                Debug.WriteLine("Validation of room layout has changed, warn title of room");
                foreach (Tile warnTile in TilesInRoom)
                {
                    if (warnTile.TileChangedActions != null)
                        warnTile.TileChangedActions(warnTile);
                }
            }
        }
    }

    ;
}