using System;

namespace Submarine.Model {

	public enum Units {
		pks,
		MWs,
		AH,
		liters_fuel,
		liters_pump,
		liters_balast,
		food,
		tins,
		Enlisted,
		// needed for generic output of Bunks
		Engineers,
		Cook,
		Watchstanders,
		Officers,
		Sonarman,
		Radioman,
		Torpedoman,
		Torpedoes,
		Radio,
		Sonar,
		Ops,
		Escape,
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

