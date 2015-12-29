using System.Collections.Generic;

namespace Submarine.Model {
	public class Generator : Room {
		

		public override Units UnitOfCapacity {
			get {
				return Units.MWs;
			}
		}


		public override bool IsLayoutValid {
			get {
				return Size >= MinimimValidSize ? true : false;
			}
		}

		public Generator (RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, List<Resource> reqRes) : base (ofThisRoomType, sub, minSize, capPerTile, reqRes) {
			IsAccessable = false;
		}
	}
}