using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicaAmbiente : MonoBehaviour
{
    // Preferible usar PascalCase para propiedad pública si lo deseas
    public static MusicaAmbiente instance;

    private AudioSource audioSource;

    [Header("Volúmenes y escena")]
    public float volumenNormal = 1f;
    public string escenaVolumenBajo = "Calderetes";
    public float volumenBajo = 0.3f;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            Debug.LogWarning("MusicaAmbiente: falta AudioSource en el mismo GameObject.");

        DontDestroyOnLoad(gameObject);

        // Suscribirse a cambios de escena
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Evitar fugas si se destruye
        if (SceneManager.GetActiveScene() != null)
            SceneManager.sceneLoaded -= OnSceneLoaded;
        if (instance == this)
            instance = null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (audioSource == null) return;

        if (scene.name == escenaVolumenBajo)
            audioSource.volume = volumenBajo;
        else
            audioSource.volume = volumenNormal;
    }

    public void PausarMusica()
    {
        if (audioSource == null)
        {
            Debug.LogWarning("PausarMusica: audioSource es null.");
            return;
        }

        if (audioSource.isPlaying)
            audioSource.Pause();
    }

    public void ReanudarMusica()
    {
        if (audioSource == null)
        {
            Debug.LogWarning("ReanudarMusica: audioSource es null.");
            return;
        }

        if (!audioSource.isPlaying)
            audioSource.UnPause();
    }

    public void SetVolume(float v)
    {
        if (audioSource == null) return;
        audioSource.volume = Mathf.Clamp01(v);
    }
}
