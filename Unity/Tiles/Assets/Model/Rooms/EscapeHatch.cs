using System.Collections.Generic;

namespace Submarine.Model {
	public class EscapeHatch : Room {
		

		public override Units UnitOfOutput {
			get {
				return Units.Escape;
			}
		}

		public override bool IsLayoutValid {
			get { // check size req.
				bool sizeOk = Size >= MinimimValidSize;
				// should be connected  to the Bride tower
				bool locationValid = false;

				int belowTopOfSub = inSub.heightOfSub - inSub.heightOfBridgeTower - 1;

				foreach (Point coord in coordinatesOfTilesInRoom) {
					Tile checkTile = inSub.GetTileAt (coord.x, coord.y);
					if (checkTile.Y == belowTopOfSub)
						locationValid = true;
				}

				return sizeOk && locationValid; // all req needs true
			}
		}

		public EscapeHatch (RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, List<Resource> reqRes) : base (ofThisRoomType, sub, minSize, capPerTile, reqRes) {
			//SetRoomValidationText ();
			ValidationText += " and must be connected to the top of the submarine.";
		}
	}
}