using System.Collections.Generic;

namespace Submarine.Model {
	public class Battery : Room {

		public override Units UnitOfOutput {
			get {
				return Units.AH;
			}
		}

		public override bool IsLayoutValid {
			get {
				return Size >= MinimimValidSize ? true : false;
			}
		}

		public Battery (RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, List<Resource> reqRes) :
			base (ofThisRoomType, sub, minSize, capPerTile, reqRes) {
			IsAccessable = false;
		}
	}
}