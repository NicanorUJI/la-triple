using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Teleport target; // El otro teletransporte
    public float offsetX = 0f;
    private bool isTeleporting = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.CompareTag("Player") || other.CompareTag("Ghost")) && !isTeleporting)
        {
            if (target == null) return;

            // Activar flag en el portal destino para evitar bucle
            target.isTeleporting = true;

            Vector3 newPos = other.transform.position;
            newPos.x = target.transform.position.x + offsetX;
            other.transform.position = newPos;

            // Activar flag local para evitar retrigger mientras sigue dentro
            isTeleporting = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Ghost"))
        {
            // Cuando el personaje sale del trigger, permitir nuevo teletransporte
            isTeleporting = false;
        }
    }
}
