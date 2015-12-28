namespace Submarine.Model {
	public class RadioRoom : Room {
		

		public override Units UnitOfCapacity {
			get {
				return Units.None;
			}
		}

		public override Units ResourceUnit {
			get {
				return Units.Radioman;
			}
		}



		public override bool IsLayoutValid {
			get {
				return Size >= MinimimValidSize ? true : false;
			}
		}

		public RadioRoom (RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, int reqRes) : base (ofThisRoomType, sub, minSize, capPerTile, reqRes) {
			IsAccessable = false;
		}
	}
}