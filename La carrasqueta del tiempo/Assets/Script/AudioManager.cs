using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Sonidos UI")]
    public AudioClip botonPausaClip;
    public AudioClip botonOpcionClip;
    public AudioClip clipLlave;
    private AudioSource audioSource;

    void Awake()
    {
        // Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // No se destruye al cambiar de escena
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    // Función para reproducir un clip
    public void Reproducir(AudioClip clip)
    {
        if (clip != null)
            audioSource.PlayOneShot(clip);
    }
}
