using Domino.Core;
using System.Collections.Generic;
using System.Linq;

namespace Domino.AI
{
    public static class NpcAgent
    {
        // Devuelve una jugada válida o null si no puede
        public static (DominoTile tile, bool toLeft)? ChooseMove(BoardState board, List<DominoTile> hand)
        {
            var moves = RulesEngine.GetValidMoves(hand, board);
            if (moves.Count == 0)
                return null;

            // Heurística básica: elige la jugada que deje el valor más frecuente en su mano en los extremos
            var best = moves.OrderByDescending(m =>
                hand.Count(t => t.A == (m.ToLeft ? board.RightValue : board.LeftValue) ||
                                t.B == (m.ToLeft ? board.RightValue : board.LeftValue))
            ).First();

            return (best.Tile, best.ToLeft);
        }
    }
}
