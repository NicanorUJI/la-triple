using System;

namespace Domino.Core
{
    /// <summary>Ficha de dominó. Inmutable.</summary>
    [Serializable]
    public readonly struct DominoTile : IEquatable<DominoTile>
    {
        public readonly int A; // valor lado 1 (0..6)
        public readonly int B; // valor lado 2 (0..6)

        public DominoTile(int a, int b)
        {
            A = a; B = b;
        }

        public bool Matches(int n) => A == n || B == n;
        public DominoTile Flipped() => new DominoTile(B, A);
        public int PipSum => A + B;

        public override string ToString() => $"{A}|{B}";

        // cambiar aqui las imagenes de la ficha ??


        public bool Equals(DominoTile other) =>
            (A == other.A && B == other.B) || (A == other.B && B == other.A);

        public override bool Equals(object obj) => obj is DominoTile t && Equals(t);
        public override int GetHashCode()
        {
            // hash independiente del orden (A|B == B|A)
            int min = Math.Min(A, B);
            int max = Math.Max(A, B);
            return (min * 31) ^ max;
        }
    }
}
