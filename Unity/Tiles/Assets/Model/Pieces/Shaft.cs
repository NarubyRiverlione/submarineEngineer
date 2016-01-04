using System;

namespace Submarine.Model
{
	public class Shaft:Piece
	{
	// override	public Units UnitOfContent {get{ return Units.pks;}}

		public Shaft (PieceType type,Tile tile,Sub inSub) : base (type,tile,inSub)
		{
		}
	}
}

