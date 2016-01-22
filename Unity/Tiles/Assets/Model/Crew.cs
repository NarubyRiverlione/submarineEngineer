using System;

namespace  Submarine.Model {
	public class Crew:Item {
		public Units Type { get; set; }

		public Crew (Units crewType, Sub sub, Point onCoord) : base (sub, onCoord) {
			Type = crewType;
		}
	}
}

