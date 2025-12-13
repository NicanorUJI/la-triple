using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Domino.Core;
using System;
using TMPro;

namespace Domino.UI
{
    public class DominoTileView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] TMP_Text label;
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] Image imageA;
        [SerializeField] Image imageB;

        [SerializeField] Sprite[] numberSprites;

        RectTransform rectTransform;
        Canvas parentCanvas;

        DominoTile _tile;
        Action<DominoTile, Vector2> _onEndDrag;
        bool _interactable;
        Vector2 _startAnchoredPos;

        RectTransform parentRect;
        Vector2 dragOffset;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();
            parentRect = transform.parent as RectTransform;
        }

        /// <summary>Setup completo con drag callback.</summary>
        public void Setup(
            DominoTile tile,
            Canvas canvas,
            Action<DominoTile, Vector2> onEndDrag,
            bool interactable = true)
        {
            _tile = tile;
            parentCanvas = canvas;
            _onEndDrag = onEndDrag;
            _interactable = interactable;

            if (label)
            {
                label.text = $"{tile.A}|{tile.B}";

                //cambiar imagen de la ficha

                imageA.sprite = numberSprites[tile.A];
                imageB.sprite = numberSprites[tile.B];
            }

            

            if (canvasGroup)
            {
                canvasGroup.blocksRaycasts = true;
                canvasGroup.alpha = interactable ? 1f : 0.4f;
            }
        }

        /// <summary>Overload cómodo para fichas de la mesa (sin drag).</summary>
        public void Setup(DominoTile tile, Canvas canvas, bool interactable = true)
        {
            Setup(tile, canvas, null, interactable);
        }

        public void SetInteractable(bool value)
        {
            _interactable = value;
            if (canvasGroup)
                canvasGroup.alpha = value ? 1f : 0.4f;
        }

        // --- Drag interfaces ---

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!_interactable) return;

            if (canvasGroup)
                canvasGroup.blocksRaycasts = false;

            if (parentRect == null)
                parentRect = transform.parent as RectTransform;

            // Posición del puntero en el espacio local del padre
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                eventData.position,
                null, // RenderMode Overlay: cámara null
                out var pointerLocalPos);

            // Guardamos la diferencia entre donde está la ficha y donde está el puntero
            dragOffset = rectTransform.anchoredPosition - pointerLocalPos;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_interactable || parentRect == null) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                eventData.position,
                null, // si usas Screen Space - Overlay; si no, usa parentCanvas.worldCamera
                out var pointerLocalPos);

            // Mantener el offset inicial para que la ficha siga al cursor
            rectTransform.anchoredPosition = pointerLocalPos + dragOffset;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_interactable) return;

            if (canvasGroup)
                canvasGroup.blocksRaycasts = true;

            _onEndDrag?.Invoke(_tile, eventData.position);
        }
    }
}
