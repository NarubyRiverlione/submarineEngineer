using System.Collections.Generic;

namespace Submarine.Model {
	public class PumpRoom : Room {
		

		public override Units UnitOfOutput {
			get {
				return Units.liters_pump;
			}
		}

		public override bool IsLayoutValid {
			get {
				return Size >= MinimimValidSize ? true : false;
			}
		}

		public PumpRoom (RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, List<Resource> reqRes) :
			base (ofThisRoomType, sub, minSize, capPerTile, reqRes) {
			IsAccessable = false;
		}
	}
}