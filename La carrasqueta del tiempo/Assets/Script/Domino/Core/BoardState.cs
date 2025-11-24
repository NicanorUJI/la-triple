using System.Collections.Generic;

namespace Domino.Core
{
    public class BoardState
    {
        public readonly List<DominoTile> Chain = new List<DominoTile>();
        public bool IsEmpty => Chain.Count == 0;

        public int LeftValue  => IsEmpty ? -1 : (Chain[0].A);
        public int RightValue => IsEmpty ? -1 : (Chain[^1].B);

        public void Clear() => Chain.Clear();

        // Coloca la primera ficha sin condiciones (usada para la salida)
        public void PlaceFirst(DominoTile tile)
        {
            Chain.Clear();
            Chain.Add(new DominoTile(tile.A, tile.B));
        }

        public bool CanPlaceLeft(DominoTile tile)
        {
            if (IsEmpty) return true;
            return tile.A == LeftValue || tile.B == LeftValue;
        }

        public bool CanPlaceRight(DominoTile tile)
        {
            if (IsEmpty) return true;
            return tile.A == RightValue || tile.B == RightValue;
        }

        public void PlaceLeft(DominoTile tile)
        {
            if (IsEmpty) { PlaceFirst(tile); return; }
            // Queremos que el LADO DERECHO de la ficha sea el valor que ya estaba a la izquierda.
            if (tile.A == LeftValue) tile = tile.Flipped(); // ahora tile.B == LeftValue
            // Si ya es compatible, insertamos como (nuevoIzq|LeftValue)
            Chain.Insert(0, new DominoTile(tile.A, tile.B));
        }

        public void PlaceRight(DominoTile tile)
        {
            if (IsEmpty) { PlaceFirst(tile); return; }
            // Queremos que el LADO IZQUIERDO de la ficha sea el valor que ya estaba a la derecha.
            if (tile.B == RightValue) tile = tile.Flipped(); // ahora tile.A == RightValue
            // Si ya es compatible, agregamos como (RightValue|nuevoDer)
            Chain.Add(new DominoTile(tile.A, tile.B));
        }
    }
}
