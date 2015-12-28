namespace Submarine.Model {
	public class EngineRoom : Room {

		public override Units UnitOfCapacity {
			get {
				return Units.pks;
			}
		}

		public override Units ResourceUnit {
			get {
				return Units.liters_fuel;
			}
		}



		public override bool IsLayoutValid {
			get {
				return Size >= MinimimValidSize ? true : false;
			}
		}

		public EngineRoom (RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, int reqRes) :
			base (ofThisRoomType, sub, minSize, capPerTile, reqRes) {
			IsAccessable = false;
		}
	}
}