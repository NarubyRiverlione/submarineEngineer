namespace Submarine.Model {
	public class Bridge : Room {
		

		public override Units UnitOfCapacity {
			get {
				return Units.None;
			}
		}

		public override Units ResourceUnit {
			get {
				return Units.Crew;
			}
		}

		public override bool IsLayoutValid {
			get {
				return Size >= MinimimValidSize ? true : false;
			}
		}

		public Bridge (RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, int reqRes) : base (ofThisRoomType, sub, minSize, capPerTile, reqRes) {
		}
	}
}