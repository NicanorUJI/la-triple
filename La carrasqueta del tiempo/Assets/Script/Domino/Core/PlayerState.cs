using System.Collections.Generic;
using System.Linq;

namespace Domino.Core
{
    public class PlayerState
    {
        public readonly List<DominoTile> Hand = new List<DominoTile>();
        public int PipSum => Hand.Sum(t => t.PipSum);

        public void Add(DominoTile t) => Hand.Add(t);
        public bool Remove(DominoTile t)
        {
            // Borrar por valor (A|B == B|A) → usamos Equals de DominoTile
            int idx = Hand.FindIndex(h => h.Equals(t));
            if (idx >= 0) { Hand.RemoveAt(idx); return true; }
            return false;
        }

        public bool HasTile(int a, int b) => Hand.Exists(t => (t.A == a && t.B == b) || (t.A == b && t.B == a));
        public bool IsEmpty => Hand.Count == 0;
    }
}
