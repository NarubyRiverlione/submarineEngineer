using System;

namespace Submarine.Model
{
	public class Pipe:Piece
	{
		//override public Units UnitOfContent {get{ return Units.liters_fuel;}}

		public Pipe (PieceType type,Tile tile,Sub inSub) : base (type,tile,inSub)
		{
		}
	}
}

