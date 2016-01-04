using System;
using System.Collections.Generic;

namespace Submarine.Model
{
	public class ChargeCable:Carrier
	{

		public ChargeCable(int id,Units unit) : base(id,unit){}

		override public void AddPiece(Piece cable){
			if (cable.GetType ()== typeof(Wire) )
				Pieces.Add (cable);
		}
		override public void RemovePiece(Piece cable){
			if (cable.GetType () == typeof(Wire)) {
				if (Pieces.Contains (cable))
					Pieces.Remove (cable);
			}
		}


	}
}

