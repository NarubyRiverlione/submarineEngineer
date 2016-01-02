using System.Collections.Generic;

namespace Submarine.Model {
	public class TorpedoRoom : Room {
		
		public override Units UnitOfOutput {
			get {
				return Units.Torpedoes; //TODO: should produce 1 Weapon Unit instead ?
			}
		}

		public override bool IsLayoutValid {
			get {
				return Size >= MinimimValidSize ? true : false;
			}
		}

		public TorpedoRoom (RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, List<Resource> reqRes) : base (ofThisRoomType, sub, minSize, capPerTile, reqRes) {
			IsAccessable = false;
		}
	}
}