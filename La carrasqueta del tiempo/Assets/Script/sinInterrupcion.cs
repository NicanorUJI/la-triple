using UnityEngine;

public class MusicaAmbiente : MonoBehaviour
{
    // 💡 Usa una instancia estática para que sea fácil acceder a ella y para
    // aplicar el patrón Singleton.
    public static MusicaAmbiente instance;

    private AudioSource audioSource;

    private void Awake()
    {
        // 1. **Implementar el patrón Singleton:**
        // Si ya existe una instancia de este script, destruye esta nueva.
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        // 2. Si no hay otra instancia, esta es la única.
        instance = this;
        audioSource = GetComponent<AudioSource>();

        // 3. **Persistir el objeto:** Evita que el objeto se destruya al cargar una nueva escena.
        DontDestroyOnLoad(this.gameObject);
    }

    // Opcional: Métodos públicos para controlar la música desde otros scripts
    public void PausarMusica()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }

    public void ReanudarMusica()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.UnPause();
        }
    }
    
    // Si la música debe empezar al cargar la primera escena y está marcada como 'Play On Awake', no necesitas el Start.
    /*
    private void Start()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
    */
}
