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

		private CrewMode _mode;

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

		public Crew (Units crewType, Sub sub, Point onCoord) : base (sub, onCoord) {
			Type = crewType;
			Mode = CrewMode.Resting;
		}

		#endregion


		private void CrewModeChanged (CrewMode oldMode, CrewMode newMode) {
			switch (newMode) {
				case CrewMode.Working:
					GotoWorkFrom (oldMode);
					break;
				case CrewMode.Eating:
					GotoEatingFrom (oldMode);
					break;
				case CrewMode.Resting:
					GotoRestFrom (oldMode);
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
			UnityEngine.Debug.Log ("Move crew to a " + targetRoomType);
			List<int> roomIDsToGoto = inSub.GetRoomIDsForRoomType (targetRoomType);
			if (roomIDsToGoto.Count == 0) {
				UnityEngine.Debug.Log ("No " + targetRoomType + " found in sub. No where to go to !");
				return;
			}

			if (roomIDsToGoto.Count == 1) {
				Room targetRoom = inSub.GetRoom (roomIDsToGoto [0]);
				List<Point> walkableCoordInTargetRoom = targetRoom.LowestCoordInRoom;
				//TODO: don't set all crew on same tile
				Point targetCoord = walkableCoordInTargetRoom [0];
				// TODO: don't beam me up scotty: crew needs to walk to target coord
				coord = targetCoord;
				UnityEngine.Debug.Log ("Crew is moved to " + coord.x + "," + coord.y + " in roomID " + targetRoom.RoomID);
			}
			else {
				//TODO: multple rooms to go to ...
				UnityEngine.Debug.Log ("Oops multiple same type of rooms to go to. Don't know witch crew should go to....");
				foreach (int roomID in roomIDsToGoto) {
					Room gotoRoom = inSub.GetRoom (roomID);
					if (gotoRoom.ResourcesAvailable == false) {
						
					}
				}

			}
		}
	}
}

