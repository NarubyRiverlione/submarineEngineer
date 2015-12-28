namespace Submarine.Model {
	public class StorageRoom : Room {
		

		public override Units UnitOfCapacity {
			get {
				return Units.tins;
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

		public StorageRoom (RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, int reqRes) : base (ofThisRoomType, sub, minSize, capPerTile, reqRes) {
			IsAccessable = false;
		}
	}
}