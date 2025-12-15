using UnityEngine;
using UnityEngine.SceneManagement;

public class ArbolInteractuable : MonoBehaviour, IInteractable
{
    public string escenaDestino;
    public AudioClip sonidoViaje;

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

        // Crear un objeto para reproducir el sonido
        GameObject audioObj = new GameObject("AudioTransicion");
        AudioSource audioSource = audioObj.AddComponent<AudioSource>();

        audioSource.clip = sonidoViaje;
        audioSource.Play();

        // Mantenerlo al cambiar de escena
        DontDestroyOnLoad(audioObj);

        // Esperar 1 segundo antes de cambiar
        yield return new WaitForSeconds(1f);

        // Cambiar de escena
        SceneManager.LoadScene(escenaDestino);

        // Destruir el objeto cuando termine el audio
        Destroy(audioObj, sonidoViaje.length);
    }
}
