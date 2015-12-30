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
		Crew,
		// needed for generic output of Bunks
		Engineers,
		Cook,
		Watchstander,
		Officers,
		//Lookouts,
		Sonarman,
		Radioman,
		Torpedoman,
		Torpedoes,
		Radio,
		Sonar,
		Ops,
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

