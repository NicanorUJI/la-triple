using UnityEngine;
using UnityEngine.UI;

public class SpriteChanger : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite spriteBloqueado;
    public Sprite spriteCompletado;

    private Image image;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    public void SetCompletado(bool completado)
    {
        if (image == null) return;
        Debug.Log("cambio de sprite");
        image.sprite = completado ? spriteCompletado : spriteBloqueado;
    }
}
