namespace Submarine.Model {
	public class Sonar : Room {
		

		public override Units UnitOfCapacity {
			get {
				return Units.None;
			}
		}

		public override Units ResourceUnit {
			get {
				return Units.Sonarman;
			}
		
		}


		public override bool IsLayoutValid {
			get {
				return Size >= MinimimValidSize ? true : false;
			}
		}

		public Sonar (RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, int reqRes) : base (ofThisRoomType, sub, minSize, capPerTile, reqRes) {
			IsAccessable = false;
		}
	}
}