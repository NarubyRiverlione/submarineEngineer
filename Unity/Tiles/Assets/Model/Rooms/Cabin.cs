namespace Submarine.Model {
	public class Cabin : Room {



		public override bool IsLayoutValid {
			get {
				return Size >= MinimimValidSize ? true : false;
			}
		}

		public Cabin (RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, string unitOfCap) : base (ofThisRoomType, sub, minSize, capPerTile, unitOfCap) {
		}
	}
}