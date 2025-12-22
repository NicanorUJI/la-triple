using UnityEngine;

public class Note : MonoBehaviour
{
    public float fallSpeed = 0.6f;
    public float destroyY = -0.2f;

    private bool canBePressed = false; // Solo se puede presionar dentro de la HitZone

    void Update()
    {
        // Movimiento constante hacia abajo
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        // Si sale de la pantalla, se destruye
        if (transform.position.y < destroyY)
        {
            Destroy(gameObject);
        }

        // Detectar pulsación solo cuando está en la zona de acierto
        if (canBePressed && Input.GetKeyDown(KeyCode.Space))
        {
            RhythmGameManager.instance.NoteHit();  // Incrementa el puntaje
            Destroy(gameObject);
        }
    }
    public bool CanBePressed()
    {
        return canBePressed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Activator"))
            canBePressed = true;  // La nota está en la zona de acierto
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Activator"))
        {
            canBePressed = false;  // La nota salió de la zona de acierto
        }
    }
}



