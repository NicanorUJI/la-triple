using UnityEngine;
using UnityEngine.UI;
using Domino.Core;
using TMPro;
using System;

namespace Domino.UI
{
    public class DominoTileView : MonoBehaviour
    {
        [SerializeField] TMP_Text label;
        [SerializeField] Button button;

        DominoTile _tile;
        Action<DominoTile> _onClick;

        public void Setup(DominoTile tile, Action<DominoTile> onClick, bool interactable = true)
        {
            _tile = tile;
            _onClick = onClick;
            if (label) label.text = $"{tile.A}|{tile.B}";
            if (button)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => _onClick?.Invoke(_tile));
                button.interactable = interactable;
            }
        }

        public void SetInteractable(bool v)
        {
            if (button) button.interactable = v;
        }
    }
}
