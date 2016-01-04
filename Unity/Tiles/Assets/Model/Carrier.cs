using System;
using System.Collections.Generic;

namespace Submarine.Model
{
	// Carrier is generic class for collection of pieces: FuelPipe contains Pipes, ChargingCable contains Wire
	abstract public class Carrier
	{
		public int ID {
			get;
			private set;
		}

		public List<Piece> Pieces{
			get;
			private set;
		}

		public int Content {
			get{ int _content = 0;
				foreach (Piece piece  in Pieces) {
					// only add correct unit to carrier
					// Ex. only add fuel as input (end of fuel pipe will be connected to Engine Room witch has pks as output, don't add pks as fuel pipe content)
					if (piece.UnitOfContent == UnitOfContent)
						_content += piece.Input;
				}
				return _content; 
			}
		}

		public Units UnitOfContent { get;private set;}



		protected Carrier (int id, Units unit) {
			ID = id;
			UnitOfContent = unit;

		}

		static public Carrier CreateCarrier(Units unit, int id){
			switch (unit) {
			case Units.liters_fuel:
				return new FuelPipe (id,unit);
			case Units.AH:
				return new ChargeCable (id,unit);

			default: 
				throw new Exception ("unknow carrier for unit "+unit);
			}

		}

		abstract public void AddPiece (Piece piece);
		abstract public void RemovePiece (Piece piece);

	}
}

