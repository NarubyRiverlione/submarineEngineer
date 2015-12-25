namespace Submarine.Model {
	public class BalastTank : Room {
		// public override RoomType TypeOfRoom { get { return RoomType.FuelTank; } }



		public override bool IsLayoutValid {
			get {
				return Size >= MinimimValidSize ? true : false;
			}
		}

		public BalastTank (RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, string unitOfCap) :
			base (ofThisRoomType, sub, minSize, capPerTile, unitOfCap) {
			IsAccessable = false;
		}
	}
}