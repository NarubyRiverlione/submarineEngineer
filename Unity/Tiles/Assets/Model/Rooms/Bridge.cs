namespace Submarine.Model
{
    public class Bridge : Room
    {
        // public override RoomType TypeOfRoom { get { return RoomType.Bridge; } }
        public override double CapacityPerTile
        {
            get
            {
                return 3.0;
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
                return 4;
            }
        }

        public override bool IsLayoutValid
        {
            get
            {
                return Size >= MinimimValidSize ? true : false;
            }
        }

        public Bridge(RoomType typeOfRoom, Sub sub): base (typeOfRoom, sub)
        {
        }
    }
}