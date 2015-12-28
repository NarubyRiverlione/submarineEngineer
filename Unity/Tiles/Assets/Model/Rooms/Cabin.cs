namespace Submarine.Model {
	public class Cabin : Room {
		

		public override Units UnitOfCapacity {
			get {
				return Units.Officer;
			}
		}

		public override Units ResourceUnit {
			get {
				return Units.food;
			}
		}




		public override bool IsLayoutValid {
			get {
				return Size >= MinimimValidSize ? true : false;
			}
		}

		public Cabin (RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, int reqRes) : base (ofThisRoomType, sub, minSize, capPerTile, reqRes) {
		}
	}
}