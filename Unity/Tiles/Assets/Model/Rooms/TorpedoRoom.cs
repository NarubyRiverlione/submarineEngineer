﻿namespace Submarine.Model
{
    public class TorpedoRoom : Room
    {


        public override bool IsLayoutValid
        {
            get
            {
                return Size >= MinimimValidSize ? true : false;
            }
        }

        public TorpedoRoom(RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, Units unitOfCap, Units resource,  int reqRes): base ( ofThisRoomType,  sub,  minSize, capPerTile,  unitOfCap, resource,  reqRes)
        {
            IsAccessable = false;
        }
    }
}