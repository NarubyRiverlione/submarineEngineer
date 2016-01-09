using System;

namespace Submarine.Model {
	public enum PieceType {
		Pipe,
		Wire,
		Shaft,
		None,
		Remove}

	;

	public class Piece {
		public Tile OnTile { get; private set; }

		[UnityEngine.SerializeField]
		Sub inSub;

		public PieceType Type { get; private set; }

		public Carrier partOfCarrier { get; set; }

		[UnityEngine.SerializeField]
		int neigboreCount;

		public int NeighboreCount {
			get { return neigboreCount; }
			set {
				neigboreCount = value;
				_isConnection = false;
				if (OnTile != null && OnTile.TileChangedActions != null)
					OnTile.TileChangedActions (OnTile);
			}
		}

		// an item can only be connected to a room, not in an empty tile
		[UnityEngine.SerializeField]
		bool _isConnection = false;

		public bool IsConnection {
			get{ return _isConnection; }
			set {
				if (OnTile.RoomID == 0 && value == true)
					UnityEngine.Debug.Log ("Cannot connect item if tile isn't part of a room");
				else
					_isConnection = true;
				// remove connection
				if (!value)
					_isConnection = false;
//				if (OnTile != null && OnTile.TileChangedActions != null)
//					OnTile.TileChangedActions (OnTile);
				partOfCarrier.WarnAllPiecesOfCarrier ();
			}
		}

		// only if an item is connected to a Room it can have input
		[UnityEngine.SerializeField]
		int _prevInput = 0;

		public int Input {
			get {
				int newInput = 0;
				if (IsConnection) {
					Room roomOfItem = inSub.GetRoom (OnTile.RoomID);
					if (roomOfItem == null)
						throw new Exception ("Cannot find room of item: roomId=" + OnTile.RoomID);
					newInput = roomOfItem.Output;
//					if (_prevInput != newInput)
//						partOfCarrier.WarnAllPiecesOfCarrier (); // input changed, warn all pieces
				}
				_prevInput = newInput;
				return newInput;
			}
		}

		public Units UnitOfContent {
			get { 
				if (IsConnection) {
					Room roomOfItem = inSub.GetRoom (OnTile.RoomID);
					if (roomOfItem == null) {
						UnityEngine.Debug.Log ("!!!! Cannot find room of item: roomId=" + OnTile.RoomID + " disconnecting piece now");
						// room doesn't exist (any more), disconnect piece
						IsConnection = false;
					}
					return roomOfItem.UnitOfOutput;

				}
				return Units.None;
			}
		}


		// CONSTRUCTOR
		public Piece (PieceType typeOfPiece, Tile tile, Sub sub) {
			Type = typeOfPiece;
			NeighboreCount = 0;
			OnTile = tile;
			inSub = sub;
			//isConnection = isconnection; // set always after setting OnTile as tile is checked to be part of a room
		}


		//		public static PieceType FindPieceType (Units units) {
		//			switch (units) {
		//				case Units.MWs:
		//					return PieceType.Wire;
		//				case Units.liters_fuel:
		//					return PieceType.Pipe;
		//				case Units.pks:
		//					return PieceType.Shaft;
		//				default:
		//					throw new Exception ("Unknow item type for unit: " + units);
		//			}
		//		}
		//
		//		public static PieceType FindPieceTypeFormString (string unitAsString) {
		//			switch (unitAsString) {
		//				case "ChargeCable":
		//					return PieceType.Wire;
		//				case "FuelPipe":
		//					return PieceType.Pipe;
		//				case "Shaft":
		//					return PieceType.Shaft;
		//				default:
		//					throw new Exception ("Unknow item type for unit: " + unitAsString);
		//			}
		//		}
	}
}

