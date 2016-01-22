using System;
using System.Collections.Generic;

namespace Submarine.Model {
	// Carrier is generic class for collection of Pieces: FuelPipe contains Pipes, ChargingCable contains Wire
	public class Carrier {
		public int ID { get; private set; }

		public List<Piece> Pieces { get; private set; }

		public int Content {
			get {
				int _content = 0;
				foreach (Piece piece  in Pieces) {
					// only add correct unit to carrier
					// Ex. only add fuel as input (end of fuel pipe will be connected to Engine Room witch has pks as output, don't add pks as fuel pipe content)
					if (piece.UnitOfContent == UnitOfContent)
						_content += piece.Input;
				}
				return _content; 
			}
		}

		public Units UnitOfContent { get; private set; }

		public List<int> connectedRoomIDs { get; private set; }


		#region CONSTRUCTOR

		public Carrier (int id, Units unit) {
			ID = id;
			UnitOfContent = unit;
			Pieces = new List<Piece> ();
			connectedRoomIDs = new List<int> ();
		}

		#endregion


		public void WarnAllPiecesOfCarrier () {
			foreach (Piece piece in Pieces) {
				if (piece.OnTile.TileChangedActions != null)
					piece.OnTile.TileChangedActions (piece.OnTile);
			}
		}

		public void AddPiece (Piece pieceToAdd) {
			Pieces.Add (pieceToAdd);
		}

		public void RemovePiece (Piece pieceToRemove) {
			int index = Pieces.FindIndex (p => p.coord.x == pieceToRemove.coord.x && p.coord.y == pieceToRemove.coord.y);
			if (index == -1) {
				UnityEngine.Debug.LogError ("cannot find piece in carrier");
				return;
			}
			Pieces.RemoveAt (index);
		}

		public bool AddConnectedRoomID (int newRoomID) {
			if (connectedRoomIDs.Contains (newRoomID)) {
				UnityEngine.Debug.Log ("Room " + newRoomID + "already connected to carrier " + ID);
				return false;
			}
			else {
				connectedRoomIDs.Add (newRoomID);
		
				return true;
			}
		}

		public bool RemoveConnectedRoomID (int newRoomID) {
			if (connectedRoomIDs.Contains (newRoomID)) {
				connectedRoomIDs.Remove (newRoomID);
				return true;
			}
			else {
				UnityEngine.Debug.Log ("ERROR: Room " + newRoomID + " isn't connected to carrier " + ID);
				return false;
			}
		}

	}
}