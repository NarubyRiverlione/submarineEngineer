using System;

namespace Submarine.Model {

	public enum Units {
		pks,
		MWs,
		AH,
		liters_fuel,
		liters_pump,
		food,
		tins,
		Engineers,
		Cook,
		Crew,
		Officers,
		Lookouts,
		Sonarman,
		Radioman,
		Torpedoman,
		Torpedoes,
		None}

	;

	public class Resource {
		public Units unit { get; set; }

		public int amount { get; set; }

		public Resource (Units u, int a) {
			unit = u;
			amount = a;

		}
	}
}

