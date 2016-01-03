using System;

namespace  Submarine.Model {
	public class Crew {
		public Units Type { get; set; }

		public int Id { get; set; }

<<<<<<< HEAD
		public Crew (Units crewType, int id) {
			Type = crewType;
			Id = id;
=======
		public Crew (Units crewType) {
			Type = crewType;

>>>>>>> 6aa99127d4653ca4e31a8d4a5401bf645e04b5bf
		}
	}
}

