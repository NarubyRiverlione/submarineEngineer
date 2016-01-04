using System;

namespace Submarine.Model
{
	public class Wire:Piece
	{
		//override public  Units UnitOfContent {get{ return Units.AH;}}

		public Wire (PieceType type,Tile tile,Sub inSub) : base (type,tile,inSub)
		{
		}
	}
}

