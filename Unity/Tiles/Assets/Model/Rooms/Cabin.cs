using System.Collections.Generic;

namespace Submarine.Model {
	public class Cabin : Room {
		

		public override Units UnitOfOutput {
			get {
				return Units.Officers;
			}
		}


		public override bool IsLayoutValid {
			get {
				return Size >= MinimimValidSize ? true : false;
			}
		}

		public Cabin (RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, List<Resource> reqRes) : base (ofThisRoomType, sub, minSize, capPerTile, reqRes) {
		}
	}
}