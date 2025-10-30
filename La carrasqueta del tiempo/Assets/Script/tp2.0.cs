using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CambioDeEscenaPorIndice : MonoBehaviour
{
    public int indiceEscena; // Ej: 1, 2, 3...

    public string tagJugador = "Player";

    public float retardo = 0f; // segundos antes de cambiar, opcional

    // 🔹 NUEVA VARIABLE para guardar el nombre del punto de entrada
    public string nombrePuntoEntrada;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(tagJugador))
        {
            // 🔹 NUEVA LÍNEA: guarda el nombre antes de cambiar de escena
            if (GameManager.Instance != null)
                GameManager.Instance.lastExitName = nombrePuntoEntrada;

            StartCoroutine(CambiarEscena());
        }
    }

    IEnumerator CambiarEscena()
    {
        yield return new WaitForSeconds(retardo);
        SceneManager.LoadScene(indiceEscena);
    }
}
