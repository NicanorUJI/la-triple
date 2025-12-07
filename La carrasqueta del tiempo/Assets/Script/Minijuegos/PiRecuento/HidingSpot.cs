using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    [HideInInspector]
    public bool hasChild = false;

    [HideInInspector]
    public GameObject childSprite; // se asigna din�micamente

    private bool found = false;
    private Renderer rend;
    private Color originalColor;
    public Color hoverColor = Color.yellow;


    private PiRecuentoManager gameManager;


    void Start()
    {
        gameManager = FindAnyObjectByType<PiRecuentoManager>();
        rend = GetComponent<Renderer>();
        if (rend != null)
            originalColor = rend.material.color;
    }

    private void OnMouseEnter()
    {
        if (rend != null && !found)
            rend.material.color = hoverColor;
    }

    private void OnMouseExit()
    {
        if (rend != null && !found)
            rend.material.color = originalColor;
    }

    private void OnMouseDown()
    {
        if (found) return;
        gameManager.OnSpotClicked(this);
    }

    public void RevealChild()
    {
        found = true;
        if (childSprite != null)
        {
            Niño n = childSprite.GetComponent<Niño>();
            if (n != null)
                n.Mostrar(); // ahora s� llamamos al m�todo de la clase Ni�o
        }
    }

    public void ResetSpot()
    {
        hasChild = false;
        found = false;

        if (childSprite != null)
        {
            Niño n = childSprite.GetComponent<Niño>();
            if (n != null)
                n.Ocultar(); // ahora s� llamamos al m�todo de la clase Ni�o
        }
    }
}




