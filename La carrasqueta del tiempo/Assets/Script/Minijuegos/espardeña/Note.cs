using UnityEngine;

public class Note : MonoBehaviour
{
    public float fallSpeed = 0.5f; // Valor por defecto más alto
    public float destroyY = -5f; // Asegúrate que este valor esté bien abajo de tu pantalla

    private bool canBePressed = false;

    void Update()
    {
        // Solo se encarga de caer
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        // Si sale de la pantalla por abajo, se destruye
        if (transform.position.y < destroyY)
        {
            // Opcional: Avisar al manager que se perdió la nota (Miss)
            if (RhythmGameManager.instance != null)
                RhythmGameManager.instance.NoteLost();

            Destroy(gameObject);
        }
    }

    // Estas funciones son leídas por el AutoBeatDetector
    public bool CanBePressed()
    {
        return canBePressed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Activator"))
            canBePressed = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Activator"))
            canBePressed = false;
    }
}