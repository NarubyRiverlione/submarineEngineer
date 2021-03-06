﻿using System.Collections.Generic;

namespace Submarine.Model {
	public class FuelTank : Room {
		

		public override Units UnitOfOutput {
			get {
				return Units.liters_fuel;
			}
		}

		public override bool IsLayoutValid {
			get {
				return Size >= MinimimValidSize ? true : false;
			}
		}

		public FuelTank (RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, List<Resource> reqRes) : base (ofThisRoomType, sub, minSize, capPerTile, reqRes) {
			IsAccessable = false;
		}
	}
}