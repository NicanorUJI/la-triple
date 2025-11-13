using UnityEngine;
using Domino.Core;

public class SanityCheck : MonoBehaviour
{
    void Start()
    {
        var t = new DominoTile(6, 2);
        Debug.Log($"Tile: {t} Matches(6)={t.Matches(6)} Matches(1)={t.Matches(1)} Flip={t.Flipped()}");
    }
}
