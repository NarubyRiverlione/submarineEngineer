namespace Submarine.Model {
	public class EmptyRoom : Room {
		// public override RoomType TypeOfRoom { get { return RoomType.FuelTank; } }



		public override bool IsLayoutValid {
			get {
				return Size >= MinimimValidSize ? true : false;
			}
		}

		public EmptyRoom (RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, string unitOfCap) :
			base (ofThisRoomType, sub, minSize, capPerTile, unitOfCap) {
			IsAccessable = false;
			ValidationText = "DESTROY a space of a room !";
		}
	}
}