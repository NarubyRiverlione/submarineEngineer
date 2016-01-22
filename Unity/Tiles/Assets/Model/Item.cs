using System;

namespace Submarine.Model {
	public class Item {
		public Point coord { get; protected set; }

		public Tile OnTile { get { return inSub.GetTileAt (coord.x, coord.y); } }

		// don't save point to Sub but set it in Load
		[FullSerializer.fsIgnore]
		public Sub inSub { get; set; }


		public Item (Sub sub, Point onCoord) {
			inSub = sub;
			coord = onCoord;
		}

	}
}

