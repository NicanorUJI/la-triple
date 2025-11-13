using System;
using System.Collections.Generic;

namespace Domino.Core
{
    /// <summary>Conjunto doble-6: 28 fichas (0..6).</summary>
    public class DominoDeck
    {
        readonly List<DominoTile> _tiles = new List<DominoTile>();
        readonly Random _rng;

        public int Count => _tiles.Count;

        public DominoDeck(int seed = 0)
        {
            _rng = seed == 0 ? new Random() : new Random(seed);
            GenerateDoubleSix();
            Shuffle();
        }

        void GenerateDoubleSix()
        {
            _tiles.Clear();
            for (int a = 0; a <= 6; a++)
                for (int b = a; b <= 6; b++) // evita duplicados (1|2 == 2|1)
                    _tiles.Add(new DominoTile(a, b));
        }

        public void Shuffle()
        {
            // Fisher–Yates
            for (int i = _tiles.Count - 1; i > 0; i--)
            {
                int j = _rng.Next(i + 1);
                var tmp = _tiles[i];
                _tiles[i] = _tiles[j];
                _tiles[j] = tmp;
            }
        }

        public bool TryDraw(out DominoTile tile)
        {
            if (_tiles.Count == 0) { tile = default; return false; }
            int last = _tiles.Count - 1;
            tile = _tiles[last];
            _tiles.RemoveAt(last);
            return true;
        }

        public IEnumerable<DominoTile> DrawMany(int count)
        {
            for (int i = 0; i < count && TryDraw(out var t); i++)
                yield return t;
        }
    }
}
