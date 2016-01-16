using System;

namespace Submarine.Model {

	public enum Units {
		kts,
		pks,
		kW,
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
		None,
		Remove}

	;

	public class Resource {
		public Units unit { get; set; }

		public float amount { get; set; }

		public Resource (Units u, float a) {
			unit = u;
			amount = a;

		}

		// enlisted = no officers (cabin), no cooks (gallery)
		static public  bool isEnlisted (Units crewType) {
			return crewType == Units.Engineers || crewType == Units.Radioman || crewType == Units.Sonarman
			|| crewType == Units.Torpedoman || crewType == Units.Watchstanders || crewType == Units.Enlisted;
		}
		// all crew types : enlisted + officers + cook
		static public bool isCrewType (Units crewType) {
			return isEnlisted (crewType) || crewType == Units.Officers || crewType == Units.Cook;
		}
	}
}

