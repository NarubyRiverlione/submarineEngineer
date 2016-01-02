using System.Collections.Generic;

namespace Submarine.Model {
	public class BalastTank : Room {

		public override Units UnitOfOutput {
			get {
				return Units.liters_balast;
			}
		}

		public override bool IsLayoutValid {
			get {
				return Size >= MinimimValidSize ? true : false;
			}
		}

		public BalastTank (RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, List<Resource> reqRes) :
			base (ofThisRoomType, sub, minSize, capPerTile, reqRes) {
			IsAccessable = false;
		}
	}
}