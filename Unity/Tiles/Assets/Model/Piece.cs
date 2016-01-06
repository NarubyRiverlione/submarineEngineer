﻿using System;

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

		Sub inSub;

		public PieceType Type { get; private set; }

		public Carrier partOfCarrier { get; set; }

		int neigboreCount;

		public int NeighboreCount {
			get { return neigboreCount; }
			set {
				neigboreCount = value;
				isConnection = false;
				if (OnTile != null && OnTile.TileChangedActions != null)
					OnTile.TileChangedActions (OnTile);
			}
		}

		// functions can registered via this Action to be informed when tile is changed
		//		public Action PieceChangedActions { get; set; }


		// an item can only be connected to a room, not in an empty tile
		bool isConnection;

		public bool IsConnection {
			get{ return isConnection; }
			set {
				if (OnTile.RoomID == 0 && value == true)
					UnityEngine.Debug.Log ("Cannot connect item if tile isn't part of a room");
				else
					isConnection = true;
				// remove connection
				if (!value)
					isConnection = false;
				if (OnTile != null && OnTile.TileChangedActions != null)
					OnTile.TileChangedActions (OnTile);
			}
		}

		// only if an item is connected to a Room it can have input
		public int Input {
			get { 
				if (IsConnection) {
					Room roomOfItem = inSub.GetRoom (OnTile.RoomID);
					if (roomOfItem == null)
						throw new Exception ("Cannot find room of item: roomId=" + OnTile.RoomID);
					return roomOfItem.Output;

				}
				return 0;
			}
		}

		public Units UnitOfContent {
			get { 
				if (IsConnection) {
					Room roomOfItem = inSub.GetRoom (OnTile.RoomID);
					if (roomOfItem == null)
						throw new Exception ("Cannot find room of item: roomId=" + OnTile.RoomID);
					return roomOfItem.UnitOfOutput;

				}
				return Units.None;
			}
		}


		// CONSTRUCTOR
		public Piece (PieceType typeOfPiece, Tile tile, Sub sub, bool isconnection) {
			Type = typeOfPiece;
			NeighboreCount = 0;
			OnTile = tile;
			inSub = sub;
			isConnection = isconnection; // set always after setting OnTile as tile is checked to be part of a room
		}


		public static PieceType FindPieceType (Units units) {
			switch (units) {
				case Units.MWs:
					return PieceType.Wire;
				case Units.liters_fuel:
					return PieceType.Pipe;
				case Units.pks:
					return PieceType.Shaft;
				default:
					throw new Exception ("Unknow item type for unit: " + units);
			}
		}

		public static PieceType FindPieceTypeFormString (string unitAsString) {
			switch (unitAsString) {
				case "ChargeCable":
					return PieceType.Wire;
				case "FuelPipe":
					return PieceType.Pipe;
				case "Shaft":
					return PieceType.Shaft;
				default:
					throw new Exception ("Unknow item type for unit: " + unitAsString);
			}
		}
	}
}

