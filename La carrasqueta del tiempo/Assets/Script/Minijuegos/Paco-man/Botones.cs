using UnityEngine;
using UnityEngine.SceneManagement;

public class CambiarEscena : MonoBehaviour
{
    [Header("Spawn (opcional)")]
    public string nombrePuntoEntrada;

    // Botón Arcade
    public void Play()
    {
        SceneManager.LoadScene("Arcade");
    }

    // Botón Bar
    public void Exit()
    {
        Debug.Log("Botón Bar pulsado. Spawn: " + nombrePuntoEntrada);

        if (!string.IsNullOrEmpty(nombrePuntoEntrada) && GameManager.Instance != null)
        {
            GameManager.Instance.lastExitName = nombrePuntoEntrada;
            Debug.Log("Spawn guardado en GameManager");
        }
        else
        {
            Debug.Log("GameManager NULL o nombre vacío");
        }

        SceneManager.LoadScene("Bar");
    }
}
