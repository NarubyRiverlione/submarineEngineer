﻿using System.Collections.Generic;

namespace Submarine.Model {
	public class Conn : Room {
		

		public override Units UnitOfOutput {
			get {
				return Units.Ops;
			}
		}

	
		public override bool IsLayoutValid {
			get { // check size req.
				bool sizeOk = Size >= MinimimValidSize;
				// should be connected  to the Bride tower
				bool locationValid = false;

				int start_X_BridgeTower = inSub.startOfBridgeTower;
				int stop_X_BridgeTower = inSub.startOfBridgeTower + inSub.lenghtOfBridgeTower;
				int below_Y_BridgeTower = inSub.heightOfSub - inSub.heightOfBridgeTower - 1;

				foreach (Point coord in coordinatesOfTilesInRoom) {
					Tile checkTile = inSub.GetTileAt (coord.x, coord.y);
					if (checkTile.X > start_X_BridgeTower && checkTile.X <= stop_X_BridgeTower && checkTile.Y == below_Y_BridgeTower)
						locationValid = true;
				}

				return sizeOk && locationValid; // all req needs true
			}
		}

		public Conn (RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, List<Resource> reqRes) : base (ofThisRoomType, sub, minSize, capPerTile, reqRes) {
			//SetRoomValidationText ();
			ValidationText += " and must be connected to the Bridge.";
		}
	}
}