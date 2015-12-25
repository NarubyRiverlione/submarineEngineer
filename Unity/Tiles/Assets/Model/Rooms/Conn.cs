namespace Submarine.Model {
	public class Conn : Room {


		int start_X_BridgeTower, stop_X_BridgeTower;
		int below_Y_BridgeTower;

		public override bool IsLayoutValid {
			get { // check size req.
				bool sizeOk = Size >= MinimimValidSize;
				// should be connected  to the Bride tower
				bool locationValid = false;
				foreach (Tile checkTile in TilesInRoom) {
					if (checkTile.X > start_X_BridgeTower && checkTile.X <= stop_X_BridgeTower && checkTile.Y == below_Y_BridgeTower)
						locationValid = true;
				}

				return sizeOk && locationValid; // all req needs true
			}
		}

		public Conn (RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, string unitOfCap) : base (ofThisRoomType, sub, minSize, capPerTile, unitOfCap) {
			start_X_BridgeTower = sub.startOfBridgeTower;
			stop_X_BridgeTower = sub.startOfBridgeTower + sub.lenghtOfBridgeTower;
			below_Y_BridgeTower = sub.heightOfSub - sub.heightOfBridgeTower - 1;
			ValidationText = base.ValidationText + " and must be connected to the Bridge.";
		}
	}
}