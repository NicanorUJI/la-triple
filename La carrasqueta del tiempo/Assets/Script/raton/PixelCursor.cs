using UnityEngine;

public class PixelCursor : MonoBehaviour
{
    public RectTransform cursorUI; // arrastra tu Image aquí
    public Vector2 hotspot = Vector2.zero; // offset si quieres ajustar el clic exacto

    void Start()
    {
        Cursor.visible = false; // ocultamos el cursor real
    }

    void Update()
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            cursorUI.parent as RectTransform,
            Input.mousePosition,
            null,
            out pos
        );

        cursorUI.localPosition = pos + hotspot;
    }
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
