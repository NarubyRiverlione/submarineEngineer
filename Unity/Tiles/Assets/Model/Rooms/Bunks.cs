using System.Collections.Generic;

namespace Submarine.Model {
	public class Bunks : Room {
		

		public override Units UnitOfOutput {
			get {
				return Units.Crew;
			}
		}


		public override bool IsLayoutValid {
			get {
				return Size >= MinimimValidSize ? true : false;
			}
		}

		public Bunks (RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, List<Resource> reqRes) : base (ofThisRoomType, sub, minSize, capPerTile, reqRes) {
		}
	}
}