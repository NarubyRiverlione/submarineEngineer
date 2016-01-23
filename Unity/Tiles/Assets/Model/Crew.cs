using System;
using System.Collections.Generic;

namespace Submarine.Model {
	public enum CrewMode {
		Working,
		Resting,
		Eating,
		Walking}

	;


	public class Crew:Item {
		public Units Type { get; set; }

		private CrewMode _mode = CrewMode.Resting;

		public CrewMode Mode {
			get{ return _mode; }
			set {
				if (value != _mode)
					CrewModeChanged (_mode, value);
				_mode = value;
			}
		}

		public RoomType targetRoomType { get; private set; }

		#region CONSTRUCTOR

		public Crew (Units crewType, Sub sub, CrewMode currentMode) : base (sub, new Point (0, 0)) { // create at dummy point, set via Mode
			Type = crewType;
			inSub = sub;

			Mode = currentMode; // also sets coord of crew
		}

		#endregion


		private void CrewModeChanged (CrewMode oldMode, CrewMode newMode) {
			switch (newMode) {
				case CrewMode.Working:
					GotoWorkFrom (oldMode);
					break;
				case CrewMode.Eating:
					//TODO remove dummy setpoint
					coord = new Point (0, 0);
					//GotoEatingFrom (oldMode);
					break;
				case CrewMode.Resting:
					//TODO remove dummy setpoint
					coord = new Point (1, 1);
					//GotoRestFrom (oldMode);
					break;
				case CrewMode.Walking:
					
					break;
			}
		}

		private void GotoWorkFrom (CrewMode oldMode) {
			switch (Type) {
				case Units.Officers:	
					targetRoomType = RoomType.Conn;
					break;
				case Units.Watchstanders:	
					targetRoomType = RoomType.Conn;
					break;
				case Units.Sonarman:
					targetRoomType = RoomType.Sonar;
					break;
				case Units.Radio:
					targetRoomType = RoomType.RadioRoom;
					break;
				case Units.Torpedoman:
					targetRoomType = RoomType.TorpedoRoom;
					break;
				case Units.Engineers:
					//TODO: oops ; multiple roomTypes to go too..
					break;
				
			}

			MoveFrom (oldMode);
		}

		private void GotoEatingFrom (CrewMode oldMode) {
			targetRoomType = RoomType.Gallery;
			MoveFrom (oldMode);
		}

		private void GotoRestFrom (CrewMode oldMode) {
			if (Type == Units.Officers)
				targetRoomType = RoomType.Cabin;
			else // enlisted
				targetRoomType = RoomType.Bunks;
			MoveFrom (oldMode);
		}

		private void MoveFrom (CrewMode oldMode) {
			UnityEngine.Debug.Log ("Move " + Type + " to a " + targetRoomType);
			List<int> roomIDsToGoto = inSub.GetRoomIDsForRoomType (targetRoomType);
			if (roomIDsToGoto.Count == 0) {
				UnityEngine.Debug.Log ("No " + targetRoomType + " found in sub. No where to go to !");
				return;
			}
			foreach (int roomID in roomIDsToGoto) {
				Room gotoRoom = inSub.GetRoom (roomID);

				Resource crewNeed = new Resource (Type, gotoRoom.GetResouceNeeded (Type));


				if (gotoRoom.NeededCrewIsInRoom (crewNeed) == false) {
					List<Point> walkableCoordInTargetRoom = gotoRoom.LowestCoordInRoom;
					//TODO: don't set all crew on same tile
					Point targetCoord = walkableCoordInTargetRoom [0];
					// TODO: don't beam me up scotty: crew needs to walk to target coord
					coord = targetCoord;
					UnityEngine.Debug.Log (Type + " is moved to " + coord.x + "," + coord.y + " in roomID " + gotoRoom.RoomID);
				}
			}

		}

	}
}

