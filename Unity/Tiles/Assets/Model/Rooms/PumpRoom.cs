namespace Submarine.Model {
	public class PumpRoom : Room {
		

		public override Units UnitOfCapacity {
			get {
				return Units.liters_pump;
			}
		}

		public override Units ResourceUnit {
			get {
				return Units.None;
			}
		}

		public override bool IsLayoutValid {
			get {
				return Size >= MinimimValidSize ? true : false;
			}
		}

		public PumpRoom (RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, int reqRes) :
			base (ofThisRoomType, sub, minSize, capPerTile, reqRes) {
			IsAccessable = false;
		}
	}
}