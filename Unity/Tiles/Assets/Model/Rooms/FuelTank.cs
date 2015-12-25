namespace Submarine.Model
{
    public class FuelTank : Room
    {
        // public override RoomType TypeOfRoom { get { return RoomType.FuelTank; } }
        public override double CapacityPerTile
        {
            get
            {
                return 1000.0;
            }
        }

        public override string UnitName
        {
            get
            {
                return "liter";
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

        public FuelTank(RoomType typeOfRoom, Sub sub): base (typeOfRoom, sub)
        {
            IsAccessable = false;
        }
    }
}