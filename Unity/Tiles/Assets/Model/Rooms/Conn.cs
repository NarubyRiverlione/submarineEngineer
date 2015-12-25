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

        int start_X_BridgeTower, stop_X_BridgeTower;
        int below_Y_BridgeTower;
        public override bool IsLayoutValid
        {
            get
            { // check size req.
                bool sizeOk = Size >= MinimimValidSize;
                // should be connected  to the Bride tower
                bool locationValid = false;
                foreach (Tile checkTile in TilesInRoom)
                {
                    if (checkTile.X > start_X_BridgeTower && checkTile.X <= stop_X_BridgeTower && checkTile.Y == below_Y_BridgeTower)
                        locationValid = true;
                }

                return sizeOk && locationValid; // all req needs true
            }
        }

        public Conn(RoomType typeOfRoom, Sub sub): base (typeOfRoom, sub)
        {
            start_X_BridgeTower = sub.startOfBridgeTower;
            stop_X_BridgeTower = sub.startOfBridgeTower+sub.lenghtOfBridgeTower;
            below_Y_BridgeTower = sub.heightOfSub-sub.heightOfBridgeTower-1;
        }

        //public override string ShowValidationRules() {
        //    return "Size of room must be more then " + MinimimValidSize + " tiles and must be connected to the Bridge.";
        //    }
    }
}