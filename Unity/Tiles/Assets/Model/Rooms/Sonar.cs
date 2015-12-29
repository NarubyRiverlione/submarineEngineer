using System.Collections.Generic;

namespace Submarine.Model {
	public class Sonar : Room {
		

		public override Units UnitOfCapacity {
			get {
				return Units.None;
			}
		}

		public override bool IsLayoutValid {
			get {
				return Size >= MinimimValidSize ? true : false;
			}
		}

		public Sonar (RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, List<Resource> reqRes) : base (ofThisRoomType, sub, minSize, capPerTile, reqRes) {
			IsAccessable = false;
		}
	}
}