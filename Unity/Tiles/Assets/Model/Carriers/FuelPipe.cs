using System;
using System.Collections.Generic;

namespace Submarine.Model
{
	public class FuelPipe:Carrier
	{
		
		public FuelPipe(int id,Units unit) : base(id,unit){}

		override public void AddPiece(Piece pipe){
			if (pipe.GetType ()== typeof( Pipe) )
			Pieces.Add (pipe);
		}
		override public void RemovePiece(Piece pipe){
			if (pipe.GetType () == typeof(Pipe)) {
				if (Pieces.Contains (pipe))
					Pieces.Remove (pipe);
			}
		}

	}
}

