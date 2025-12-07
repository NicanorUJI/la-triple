using UnityEngine;

public class Niño : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Vector3 originalScale;

    private SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
    }
    public void Mostrar()
    {
        if (sr != null)
        {
            sr.sortingOrder = 10; // delante de todo
        }

        transform.SetParent(null); // sacar del spot
        transform.localScale = originalScale;

        // Mover al centro de la cámara visible (ajusta Z según tu cámara)
        Vector3 camPos = Camera.main.transform.position;
        transform.position = new Vector3(camPos.x, camPos.y, 0);
    }


    public void Ocultar()
    {
        transform.SetParent(null);
        if (sr != null)
        {
            sr.sortingOrder = -1;
            transform.localScale = originalScale;
        }
    }
}

    

