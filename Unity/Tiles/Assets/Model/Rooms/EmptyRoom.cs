namespace Submarine.Model {
	public class EmptyRoom : Room {
		// public override RoomType TypeOfRoom { get { return RoomType.FuelTank; } }



		public override bool IsLayoutValid {
			get {
				return Size >= MinimimValidSize ? true : false;
			}
		}

		public EmptyRoom (RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, Units unitOfCap, Units resource,  int reqRes) :
			base (ofThisRoomType, sub, minSize, capPerTile, unitOfCap, resource,  reqRes) {
			IsAccessable = false;
			ValidationText = "DESTROY a space of a room !";
		}
	}
}