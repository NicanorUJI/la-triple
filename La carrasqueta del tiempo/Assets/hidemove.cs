using UnityEngine;
using UnityEngine.UI;

public class Hide_notShow_IfOnTrigger : MonoBehaviour
{
    public GameObject toHide;
    public bool hide = false;

    public string[] requiredFlags;      // Flags que deben estar activos
    public string[] forbiddenFlags;     // Flags que NO deben estar activos

    private SpriteRenderer spriteRenderer;
    private Graphic uiGraphic; // Para UI (Image, Text, etc.)

    void Start()
    {
        if (toHide == null) return;

        // Intentamos obtener un SpriteRenderer
        spriteRenderer = toHide.GetComponent<SpriteRenderer>();

        // Intentamos obtener un componente UI
        uiGraphic = toHide.GetComponent<Graphic>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (!CheckFlags())
            return;

        if (toHide == null)
            return;

        if (hide)
        {
            SetAlpha(0f); // Invisible
        }
        else
        {
            SetAlpha(1f); // Visible
        }
    }

    private void SetAlpha(float alpha)
    {
        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = alpha;
            spriteRenderer.color = c;
        }

        if (uiGraphic != null)
        {
            Color c = uiGraphic.color;
            c.a = alpha;
            uiGraphic.color = c;
        }
    }

    private bool CheckFlags()
    {
        if (requiredFlags != null)
        {
            foreach (var flag in requiredFlags)
            {
                if (!string.IsNullOrEmpty(flag) && !GameManager.Check(flag))
                    return false;
            }
        }

        if (forbiddenFlags != null)
        {
            foreach (var flag in forbiddenFlags)
            {
                if (!string.IsNullOrEmpty(flag) && GameManager.Check(flag))
                    return false;
            }
        }

        return true;
    }
}
