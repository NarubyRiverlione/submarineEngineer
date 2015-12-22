
namespace Submarine.Model {
	public class Tile {
		public int X { get; private set; }

		public int Y { get; private set; }

		public int roomID { get; set; }

		public bool canContainRoom { get; set; }
		// used to exclude spaces that are outside the outline of the sub

		public Tile (int x, int y) {
			X = x;
			Y = y;
			roomID = 0;
			canContainRoom = true;
		}
	}
}
