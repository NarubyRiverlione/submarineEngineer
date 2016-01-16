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
		public Point coord { get; protected set; }

		public Tile OnTile { get { return inSub.GetTileAt (coord.x, coord.y); } }

		// don't save point to Sub but set it in Load
		[FullSerializer.fsIgnore]
		public Sub inSub { get; set; }

		public PieceType Type { get; private set; }

		Carrier partOfCarrier { 
			get { 
				if (inSub.ResourceCarriers.ContainsKey (carrierID))
					return inSub.ResourceCarriers [carrierID];
				else {
					UnityEngine.Debug.LogError ("Carrier : " + carrierID + " unknown in sub");
					return null;
				}
			}
		}

		public int carrierID { get; set; }

		[UnityEngine.SerializeField]
		int _neigboreCount = 0;

		public int NeighboreCount {
			get { return _neigboreCount; }
			set {
				_neigboreCount = value;

				// end pieces (only 1 other neighbore piece) is automaticly an connection (if it's the first in this room)
				if (_neigboreCount == 8 || _neigboreCount == 4 || _neigboreCount == 2 || _neigboreCount == 1)
					IsConnection = true;
				else
					IsConnection = false;

				if (OnTile != null && OnTile.TileChangedActions != null && carrierID != 0)
					OnTile.TileChangedActions (OnTile);
			}
		}

		// an item can only be connected to a room, not in an empty tile
		[UnityEngine.SerializeField]
		bool _isConnection = false;

		public bool IsConnection {
			get{ return _isConnection; }
			set {
				bool prev = _isConnection;
				if (value) {
					if (OnTile.RoomID == 0) {
						//UnityEngine.Debug.Log ("Cannot connect item if tile isn't part of a room");
						_isConnection = false;
					}
					else {
						//only 1 connection in same room, returns false if ther is already a connection so this is not a connection
						// don't connect not existing rooms : maybee piece is on a tile that isn't part of a room
						if (partOfCarrier != null && OnTile.RoomID != 0)
							_isConnection = partOfCarrier.AddConnectedRoomID (OnTile.RoomID);
//						else
//							_isConnection = true;
					}
				}
				// remove connection
				else {
					_isConnection = false;
					//  as there is only 1 connection on 1 room this was the last so remove this room as connected,
					if (partOfCarrier != null && prev == true && OnTile.RoomID != 0)
						partOfCarrier.RemoveConnectedRoomID (OnTile.RoomID); 
				}

				// Warn all pieces in this carrier that connection and so content has changed
				if (partOfCarrier != null && prev != _isConnection)
					partOfCarrier.WarnAllPiecesOfCarrier ();
			}
		}


		public int Input {
			get {
				int newInput = 0;
				if (IsConnection) {
					Room roomOfItem = inSub.GetRoom (OnTile.RoomID);
					if (roomOfItem != null) // carrier can pass through tile that isn't part of a room
					newInput = roomOfItem.Output;
				}
				return newInput;
			}
		}

		public Units UnitOfContent {
			get { 
				if (IsConnection) {
					Room roomOfItem = inSub.GetRoom (OnTile.RoomID);
					if (roomOfItem == null) {
						//UnityEngine.Debug.Log ("!!!! Cannot find room of item: roomId=" + OnTile.RoomID + " disconnecting piece now");
						// room doesn't exist (any more), disconnect piece
						IsConnection = false;
						return Units.None;
					}
					return roomOfItem.UnitOfOutput;
				}
				return Units.None;
			}
		}



		#region CONSTRUCTOR

		public Piece (PieceType typeOfPiece, Point onCoord, Sub sub) {
			inSub = sub;
			coord = onCoord;
			Type = typeOfPiece;
		}

		#endregion

		public void Reset () {
			Type = PieceType.None;

			carrierID = 0;
			_isConnection = false;
			_neigboreCount = 0;
			inSub = null;
		}

		public static PieceType FindPieceType (Units units) {
			switch (units) {
				case Units.MWs:
					return PieceType.Wire;
				case Units.liters_fuel:
					return PieceType.Pipe;
				case Units.liters_pump:
					return PieceType.Pipe;
				case Units.pks:
					return PieceType.Shaft;
				default:
					UnityEngine.Debug.LogError ("Unknow item type for unit: " + units);
					return PieceType.None;
			}
		}

	}
}