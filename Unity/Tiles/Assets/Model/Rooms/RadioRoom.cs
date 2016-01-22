using System.Collections.Generic;

namespace Submarine.Model {
	public class RadioRoom : Room {
		

		public override Units UnitOfOutput {
			get {
				return Units.Radio;
			}
		}

		public override bool IsLayoutValid {
			get {
				return Size >= MinimimValidSize ? true : false;
			}
		}

		public RadioRoom (RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, List<Resource> reqRes) : base (ofThisRoomType, sub, minSize, capPerTile, reqRes) {
			
		}
	}
}