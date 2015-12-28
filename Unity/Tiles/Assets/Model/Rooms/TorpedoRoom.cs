namespace Submarine.Model {
	public class TorpedoRoom : Room {
		

		public override Units UnitOfCapacity {
			get {
				return Units.Torpedoes;
			}
		}

		public override Units ResourceUnit {
			get {
				return Units.TorpedoMan;
			}
		}



		public override bool IsLayoutValid {
			get {
				return Size >= MinimimValidSize ? true : false;
			}
		}

		public TorpedoRoom (RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, int reqRes) : base (ofThisRoomType, sub, minSize, capPerTile, reqRes) {
			IsAccessable = false;
		}
	}
}