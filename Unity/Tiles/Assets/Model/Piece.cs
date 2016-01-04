using System;

namespace Submarine.Model
{
	public enum PieceType {
		Pipe,
		Wire,
		Shaft
	};

	public abstract class Piece
	{
		public Tile OnTile {
			get;
			private set;
		}

		Sub inSub;

		public PieceType Type {
			get;
			private set;
		}

		// an item can only be connected to a room, not in an empty tile
		bool isConnection;
		public bool IsConnection {
			get{ return isConnection; }
			set{if (OnTile.RoomID == 0 && value == true)
				UnityEngine.Debug.Log ("Cannot connect item if tile isn't part of a room");
				else
				isConnection = true;
				// remove connection
				if (!value)
					isConnection = false;
			}
		}

		// only if an item is connected to a Room it can have input
		public int Input {
			get{ 
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
			get{ 
				if (IsConnection) {
					Room roomOfItem = inSub.GetRoom (OnTile.RoomID);
					if (roomOfItem == null)
						throw new Exception ("Cannot find room of item: roomId=" + OnTile.RoomID);
					return roomOfItem.UnitOfOutput;

				}
				return Units.None;
			}
		}


		protected Piece (PieceType type, Tile tile,Sub sub){
			Type=type;
			OnTile=tile;
			inSub = sub;
		}

		// factory
		public static Piece CreateItem(PieceType type, Point coordOfTile,Sub inSub) {

			Tile onTile = inSub.GetTileAt (coordOfTile.x, coordOfTile.y);
			if (onTile == null) throw new Exception ("Cannot create an item on ("+coordOfTile.x+","+coordOfTile.y+"): cannot find tile in submarine");
				
			switch (type) {
			case PieceType.Wire:
				return new Wire (type,onTile,inSub);
			case PieceType.Pipe:
				return new Pipe (type,onTile,inSub);
			case PieceType.Shaft:
				return new Shaft (type,onTile,inSub);
			default:
				throw new Exception("Unknow item type:" + type);

			}
		}


	}
}

