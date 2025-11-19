using System;
using UnityEngine;

public class Note : MonoBehaviour
{
    public float fallSpeed = 3f;
    public float destroyY = -5f;

    private bool canBePressed = false; // solo se puede presionar dentro de la HitZone

    void Update()
    {
        // movimiento constante hacia abajo
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        // si sale de pantalla, se destruye
        if (transform.position.y < destroyY)
        {
            Destroy(gameObject);
        }

        // detectar pulsación solo cuando está en la zona de acierto
        if (canBePressed && Input.GetKeyDown(KeyCode.Space))
        {
            RhythmGameManager.instance.NoteHit();
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Activator"))
            canBePressed = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Activator"))
        {
            if (canBePressed)
            canBePressed = false;
        }
    }

}
