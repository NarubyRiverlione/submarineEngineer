namespace Submarine.Model {
	public class Bridge : Room {

		public override bool IsLayoutValid {
			get {
				return Size >= MinimimValidSize ? true : false;
			}
		}

		public Bridge (RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, string unitOfCap) : base (ofThisRoomType, sub, minSize, capPerTile, unitOfCap) {
		}
	}
}