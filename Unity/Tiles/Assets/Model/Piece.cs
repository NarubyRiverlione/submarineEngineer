using System;

namespace Submarine.Model {
	public enum PieceType {
		Pipe,
		Wire,
		Shaft,
		None,
		Remove}

	;

	public class Piece :Item {
		
		public PieceType Type { get; private set; }

		Carrier partOfCarrier { 
			get { 
				if (inSub.ResourceCarriers.ContainsKey (carrierID))
					return inSub.ResourceCarriers [carrierID];
				else {
					// only error if carrierID isn't null 
					// partOfCarrier is called when creating first piece in a carrier as this is default a connection
					// but that first piece is at creation not yet assigned to a carrier
					if (carrierID != 0)
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

//				// end pieces (only 1 other neighbore piece) is automaticly an connection (if it's the first in this room)
//				if (_neigboreCount == 8 || _neigboreCount == 4 || _neigboreCount == 2 || _neigboreCount == 1)
//					SetAsConnection ();
//				else
//					RemoveConnection ();

				if (OnTile != null && OnTile.TileChangedActions != null && carrierID != 0)
					OnTile.TileChangedActions (OnTile);
			}
		}

		// an item can only be connected to a room, not in an empty tile
		[UnityEngine.SerializeField]
		bool _isConnection = false;

		public bool IsConnection {
			get{ return _isConnection; }
			private set { _isConnection = value; }
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
						// room doesn't exist (any more), disconnect piece
						UnityEngine.Debug.Log ("!!!! Cannot find room of item: roomId=" + OnTile.RoomID + " disconnecting piece now");
						RemoveConnection ();
						return Units.None;
					}
					return roomOfItem.UnitOfOutput;
				}
				return Units.None;
			}
		}



		#region CONSTRUCTOR

		public Piece (PieceType typeOfPiece, Point onCoord, Sub sub) : base (sub, onCoord) {
			Type = typeOfPiece;
		}

		#endregion

		public void Reset () {
			Type = PieceType.None;

			carrierID = 0;
			//_isConnection = false;
			_neigboreCount = 0;
			inSub = null;
		}

		public static PieceType FindPieceType (Units units) {
			switch (units) {
				case Units.kW:
					return PieceType.Wire;
				case Units.liters_fuel:
					return PieceType.Pipe;
				case Units.liters_pump:
					return PieceType.Pipe;
				case Units.pks:
					return PieceType.Shaft;
				case Units.None:
					return PieceType.None;
				default:
					UnityEngine.Debug.LogError ("Unknow item type for unit: " + units);
					return PieceType.None;
			}
		}

		public void SetAsConnection () {
			if (OnTile.RoomID == 0) {
				//UnityEngine.Debug.Log ("Cannot connect item if tile isn't part of a room");
				IsConnection = false;
				return;
			}
			//only 1 connection in same room, returns false if ther is already a connection so this is not a connection
			// don't connect not existing rooms : may be piece is on a tile that isn't part of a room
			if (partOfCarrier != null && OnTile.RoomID != 0) {
				IsConnection = partOfCarrier.AddConnectedRoomID (OnTile.RoomID);
				partOfCarrier.WarnAllPiecesOfCarrier ();
			}
		}

		public void RemoveConnection () {
			IsConnection = false;
			//  as there is only 1 connection on 1 room this was the last so remove this room as connected,
			if (partOfCarrier != null && OnTile.RoomID != 0)
				partOfCarrier.RemoveConnectedRoomID (OnTile.RoomID); 
					
			// Warn all pieces in this carrier that connection and so content has changed
			if (partOfCarrier != null)
				partOfCarrier.WarnAllPiecesOfCarrier ();
		}
	}
}