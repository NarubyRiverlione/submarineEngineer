using System;
using System.Collections.Generic;

namespace Submarine.Model {
	// Carrier is generic class for collection of Pieces: FuelPipe contains Pipes, ChargingCable contains Wire
	abstract public class Carrier {
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

		public List<int> connectenRoomIDs { get; private set; }


		protected Carrier (int id, Units unit) {
			ID = id;
			UnitOfContent = unit;
			Pieces = new List<Piece> ();
			connectenRoomIDs = new List<int> ();
		}

		static public Carrier CreateCarrier (PieceType typeOfPiece, int id) {
			switch (typeOfPiece) {
				case PieceType.Pipe:
					return new FuelPipe (id, Units.liters_fuel);
				case PieceType.Wire:
					return new ChargeCable (id, Units.MWs);

				default: 
					throw new Exception ("unknow carrier for piece type: " + typeOfPiece);
			}

		}

		public void WarnAllPiecesOfCarrier () {
			foreach (Piece piece in Pieces) {
				if (piece.OnTile.TileChangedActions != null)
					piece.OnTile.TileChangedActions (piece.OnTile);
			}
		}

		abstract public void AddPiece (Piece piece);

		abstract public void RemovePiece (Piece piece);

		public bool AddConnectedRoomID (int newRoomID) {
			if (connectenRoomIDs.Contains (newRoomID)) {
				UnityEngine.Debug.Log ("Room " + newRoomID + "already connected to carrier " + ID);
				return false;
			}
			else {
				connectenRoomIDs.Add (newRoomID);
				return true;
			}
		}

		public bool RemoveConnectedRoomID (int newRoomID) {
			if (connectenRoomIDs.Contains (newRoomID)) {
				connectenRoomIDs.Remove (newRoomID);
				return true;
			}
			else {
				UnityEngine.Debug.Log ("ERROR: Room " + newRoomID + " isn't connected to carrier " + ID);
				return false;
			}
		}

	}
}

