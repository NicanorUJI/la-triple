using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CambioDeEscenaPorIndice : MonoBehaviour
{
    public int indiceEscena; // Ej: 1, 2, 3...

    public string tagJugador = "Player";

    public float retardo = 0f; // segundos antes de cambiar, opcional

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(tagJugador))
        {
            StartCoroutine(CambiarEscena());
        }
    }

    IEnumerator CambiarEscena()
    {
        yield return new WaitForSeconds(retardo);
        SceneManager.LoadScene(indiceEscena);
    }
}
