namespace Submarine.Model {
	public class Battery : Room {


		

		public override Units UnitOfCapacity {
			get {
				return Units.AH;
			}
		}

		public override Units ResourceUnit {
			get {
				return	 Units.MWs;
			}
		}



		public override bool IsLayoutValid {
			get {
				return Size >= MinimimValidSize ? true : false;
			}
		}

		public Battery (RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, int reqRes) :
			base (ofThisRoomType, sub, minSize, capPerTile, reqRes) {
			IsAccessable = false;
		}
	}
}