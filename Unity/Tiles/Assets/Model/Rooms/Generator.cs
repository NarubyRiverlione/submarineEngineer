namespace Submarine.Model {
	public class Generator : Room {
		

		public override Units UnitOfCapacity {
			get {
				return Units.MWs;
			}
		}

		public override Units ResourceUnit {
			get {
				return Units.pks;
			}
		}



		public override bool IsLayoutValid {
			get {
				return Size >= MinimimValidSize ? true : false;
			}
		}

		public Generator (RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, int reqRes) : base (ofThisRoomType, sub, minSize, capPerTile, reqRes) {
			IsAccessable = false;
		}
	}
}