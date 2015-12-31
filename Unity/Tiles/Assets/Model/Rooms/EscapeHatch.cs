using System.Collections.Generic;

namespace Submarine.Model {
	public class EscapeHatch : Room {
		

		public override Units UnitOfOutput {
			get {
				return Units.None;
			}
		}

		public override bool IsLayoutValid {
			get {
				return Size >= MinimimValidSize ? true : false;
			}
		}

		public EscapeHatch (RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, List<Resource> reqRes) : base (ofThisRoomType, sub, minSize, capPerTile, reqRes) {
			IsAccessable = false;
		}
	}
}