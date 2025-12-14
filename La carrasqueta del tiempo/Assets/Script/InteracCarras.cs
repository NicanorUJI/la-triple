using UnityEngine;
using UnityEngine.SceneManagement;

public class ArbolInteractuable : MonoBehaviour, IInteractable
{
    public string escenaDestino;
    public AudioSource audioViaje;   // <- AudioSource existente en la escena

    public bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {
        StartCoroutine(ViajarConAudio());
    }

    private System.Collections.IEnumerator ViajarConAudio()
    {
        Debug.Log("Viajando en el tiempo...");

        if (audioViaje == null)
        {
            Debug.LogError("No se ha asignado un AudioSource en la Inspector!");
            yield break;
        }

        // Reproducir audio
        audioViaje.Play();

        // Mantenerlo al cambiar de escena
        DontDestroyOnLoad(audioViaje.gameObject);

        // Esperar 1 segundo antes de cambiar
        yield return new WaitForSeconds(1f);

        // Cambiar de escena
        SceneManager.LoadScene(escenaDestino);

        // Destruir el objeto cuando termine el audio
        Destroy(audioViaje.gameObject, audioViaje.clip.length);
    }
}
