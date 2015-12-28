﻿namespace Submarine.Model {
	public class Bunks : Room {
		

		public override Units UnitOfCapacity {
			get {
				return Units.Crew;
			}
		}

		public override Units ResourceUnit {
			get {
				return Units.food;
			}
		}



		public override bool IsLayoutValid {
			get {
				return Size >= MinimimValidSize ? true : false;
			}
		}

		public Bunks (RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, int reqRes) : base (ofThisRoomType, sub, minSize, capPerTile, reqRes) {
		}
	}
}