using System;

namespace  Submarine.Model {
	public class Crew {
		public Units Type { get; set; }

		public int Id { get; set; }

		public Crew (Units crewType) {
			Type = crewType;

		}
	}
}

