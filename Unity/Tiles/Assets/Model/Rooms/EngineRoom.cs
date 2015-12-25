namespace Submarine.Model {
	public class EngineRoom : Room {


		public override bool IsLayoutValid {
			get {
				return Size >= MinimimValidSize ? true : false;
			}
		}

		public EngineRoom (RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, string unitOfCap) :
			base (ofThisRoomType, sub, minSize, capPerTile, unitOfCap) {
			IsAccessable = false;
		}
	}
}