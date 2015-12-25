namespace Submarine.Model
{
    public class Cabin : Room
    {
        //  public override RoomType TypeOfRoom { get { return RoomType.Cabin; } }
        public override double CapacityPerTile
        {
            get
            {
                return 2;
            }
        }

        public override string UnitName
        {
            get
            {
                return "officers";
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

        public Cabin(RoomType typeOfRoom, Sub sub): base (typeOfRoom, sub)
        {
        }
    }
}