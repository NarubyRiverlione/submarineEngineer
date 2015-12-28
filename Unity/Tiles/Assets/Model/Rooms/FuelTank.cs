namespace Submarine.Model {
	public class FuelTank : Room {
		

		public override Units UnitOfCapacity {
			get {
				return Units.liters_fuel;
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

		public FuelTank (RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, int reqRes) : base (ofThisRoomType, sub, minSize, capPerTile, reqRes) {
			IsAccessable = false;
		}
	}
}