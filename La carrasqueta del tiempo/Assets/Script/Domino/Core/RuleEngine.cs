using System.Collections.Generic;
using System.Linq;

namespace Domino.Core
{
    public static class RulesEngine
    {
        public struct Move
        {
            public DominoTile Tile;
            public bool ToLeft; // true = colocar a la izquierda; false = derecha
            public Move(DominoTile t, bool left) { Tile = t; ToLeft = left; }
        }

        public static List<Move> GetValidMoves(IEnumerable<DominoTile> hand, BoardState board)
        {
            var moves = new List<Move>();
            if (board.IsEmpty)
            {
                // Si la mesa está vacía, cualquier ficha vale (podemos empezar por derecha)
                foreach (var t in hand) moves.Add(new Move(t, false));
                return moves;
            }

            int L = board.LeftValue, R = board.RightValue;
            foreach (var t in hand)
            {
                if (t.A == L || t.B == L) moves.Add(new Move(t, true));
                if (t.A == R || t.B == R) moves.Add(new Move(t, false));
            }
            return moves;
        }

        public static void Apply(BoardState board, ref DominoTile tile, bool toLeft)
        {
            if (toLeft) board.PlaceLeft(tile);
            else        board.PlaceRight(tile);
        }
    }
}
