using System.Collections.Generic;

namespace Submarine.Model {
	public class Propellor : Room {

		public override Units UnitOfOutput {
			get {
				return Units.kts;
			}
		}

		public override bool IsLayoutValid {
			get {
				return Size >= MinimimValidSize ? true : false;
			}
		}

		public Propellor (RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, List<Resource> reqRes) :
			base (ofThisRoomType, sub, minSize, capPerTile, reqRes) {
			IsAccessable = false;
		}
	}
}