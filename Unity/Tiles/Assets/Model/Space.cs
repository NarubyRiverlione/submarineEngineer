
namespace Submarine {
    public class Space {
		public int X {
			get;
			private set;
		}
		public int Y {
			get;
			private set;
		}
        public int roomID { get;  set; } = 0;

        public bool canContainRoom { get;  set; } = true;    // used to exclude spaces that are outside the outline of the sub

		public Space (int x, int y) {
			X = x;
			Y = y;
		}
    }
}
