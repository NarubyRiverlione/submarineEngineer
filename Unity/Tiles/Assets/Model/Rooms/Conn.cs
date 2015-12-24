namespace Submarine.Model
{
    public class Conn : Room
    {
        // public override RoomType TypeOfRoom { get { return RoomType.Bridge; } }
        public override double CapacityPerTile
        {
            get
            {
                return 6;
            }
        }

        public override string UnitName
        {
            get
            {
                return "crew";
            }
        }

        public override int MinimimValidSize
        {
            get
            {
                return 6;
            }
        }

        int startValid_X, stopValid_X;
        int valid_Y;
        public override bool IsLayoutValid
        {
            get
            { // check size req.
                bool sizeOk = Size >= MinimimValidSize;
                // should be connected  to the Bride tower
                bool locationValid = false;
                foreach (Tile checkTile in TilesInRoom)
                {
                    if (checkTile.X > startValid_X && checkTile.X <= stopValid_X && checkTile.Y == valid_Y)
                        locationValid = true;
                }

                return sizeOk && locationValid; // all req needs true
            }
        }

        public Conn(RoomType typeOfRoom, Sub sub): base (typeOfRoom, sub)
        {
            startValid_X = sub.lengthOfSub / 3 + 2;
            stopValid_X = startValid_X + sub.lenghtOfBridgeTower;
            valid_Y = sub.heightOfBridgeTower + 1;
        }
    }
}