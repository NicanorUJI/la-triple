using UnityEngine;
using UnityEngine.SceneManagement;

public class CambiarEscena : MonoBehaviour
{
    [Header("Spawn (opcional)")]
    public string nombrePuntoEntrada;

    // +++ NUEVO: Variables de Audio +++
    [Header("Audio")]
    public AudioSource audioSource;      // Arrastra aquí el componente Audio Source
    public AudioClip sonidoGameOver;     // Arrastra aquí el sonido de "Perdiste"
    // +++++++++++++++++++++++++++++++++

    // +++ NUEVO: Start se ejecuta al iniciar la escena +++
    private void Start()
    {
        // Si hemos asignado el sonido y el audio source, lo reproducimos una vez
        if (audioSource != null && sonidoGameOver != null)
        {
            audioSource.PlayOneShot(sonidoGameOver);
        }
    }
    // ++++++++++++++++++++++++++++++++++++++++++++++++++++

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