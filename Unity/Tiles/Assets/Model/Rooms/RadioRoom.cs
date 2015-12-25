namespace Submarine.Model
{
    public class RadioRoom : Room
    {


        public override bool IsLayoutValid
        {
            get
            {
                return Size >= MinimimValidSize ? true : false;
            }
        }

        public RadioRoom(RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, string unitOfCap): base ( ofThisRoomType,  sub,  minSize, capPerTile,  unitOfCap)
        {
            IsAccessable = false;
        }
    }
}