namespace Submarine.Model
{
    public class Bunks : Room
    {
        // public override RoomType TypeOfRoom { get { return RoomType.Bridge; } }
        public override double CapacityPerTile
        {
            get
            {
                return 12.0;
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

        public override bool IsLayoutValid
        {
            get
            {
                return Size >= MinimimValidSize ? true : false;
            }
        }

        public Bunks(RoomType typeOfRoom, Sub sub): base (typeOfRoom, sub)
        {
        }
    }
}