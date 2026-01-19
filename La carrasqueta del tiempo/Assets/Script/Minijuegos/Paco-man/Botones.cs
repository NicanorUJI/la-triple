using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Necesario para IEnumerator

public class CambiarEscena : MonoBehaviour
{
    [Header("Spawn (opcional)")]
    public string nombrePuntoEntrada;

    [Header("Audio Config")]
    public AudioSource audioSource;      
    public AudioClip sonidoGameOver;     // Sonido al iniciar (ya lo tenías)
    
    // +++ NUEVO: Sonido de clic y tiempo de espera +++
    public AudioClip sonidoBoton;        // Arrastra aquí el sonido del "Clic"
    [Range(0.1f, 2.0f)]
    public float tiempoEspera = 0.5f;    // Tiempo en segundos antes de cambiar escena
    // ++++++++++++++++++++++++++++++++++++++++++++++++

    private void Start()
    {
        // Tu lógica original de Start se mantiene igual
        if (audioSource != null && sonidoGameOver != null)
        {
            audioSource.PlayOneShot(sonidoGameOver);
        }
    }

    // --- MODIFICADO: Botón Arcade ---
    public void Play()
    {
        // Iniciamos la rutina de espera enviando el nombre de la escena
        
        StartCoroutine(CambiarEscenaConRetraso("Arcade"));
    }

    // --- MODIFICADO: Botón Bar ---
    public void Exit()
    {
        Debug.Log("Botón Bar pulsado. Spawn: " + nombrePuntoEntrada);

        // Guardamos los datos PRIMERO (antes de esperar)
        if (!string.IsNullOrEmpty(nombrePuntoEntrada) && GameManager.Instance != null)
        {
            GameManager.Instance.lastExitName = nombrePuntoEntrada;
            Debug.Log("Spawn guardado en GameManager");
        }
        else
        {
            Debug.Log("GameManager NULL o nombre vacío");
        }

        // Luego iniciamos la rutina de espera
        StartCoroutine(CambiarEscenaConRetraso("Bar"));
    }

    // +++ NUEVO: La Corrutina que hace la magia +++
    IEnumerator CambiarEscenaConRetraso(string nombreEscena)
    {
        // 1. Reproducir sonido si existe
        if (audioSource != null && sonidoBoton != null)
        {
            audioSource.PlayOneShot(sonidoBoton);
        }

        // 2. Esperar el tiempo definido (puedes ajustar 'tiempoEspera' en el inspector)
        // Nota: WaitForSeconds usa tiempo real, no microsegundos, para que sea perceptible.
        yield return new WaitForSeconds(tiempoEspera);

        // 3. Cargar la escena
        SceneManager.LoadScene(nombreEscena);
    }
}