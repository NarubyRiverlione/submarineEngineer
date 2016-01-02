//using System;

namespace Submarine.Model {

	// Point type is in .NET 4.5 in the System.Windows namespace but this namespace isn't in Unity
	public struct Point {
		public int x, y;

		public Point (int px, int py) {
			x = px;
			y = py;
		}
	}
}

