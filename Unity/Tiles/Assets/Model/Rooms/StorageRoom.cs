using System.Collections.Generic;

namespace Submarine.Model {
	public class StorageRoom : Room {
		

		public override Units UnitOfCapacity {
			get {
				return Units.tins;
			}
		}

		public override bool IsLayoutValid {
			get {
				return Size >= MinimimValidSize ? true : false;
			}
		}

		public StorageRoom (RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, List<Resource> reqRes) : base (ofThisRoomType, sub, minSize, capPerTile, reqRes) {
			IsAccessable = false;
		}
	}
}