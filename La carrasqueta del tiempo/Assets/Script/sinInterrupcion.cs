using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MusicaAmbiente : MonoBehaviour
{
    public static MusicaAmbiente instance;

    private AudioSource audioSource;

    [Header("AudioMixer")]
    public AudioMixer audioMixer;

    private const string VOLUME_KEY = "savedMasterVolume";

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        AplicarVolumenGuardado();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AplicarVolumenGuardado();
    }

    private void AplicarVolumenGuardado()
    {
        float saved = PlayerPrefs.GetFloat(VOLUME_KEY, 100f);

        if (saved < 1) saved = 0.001f;

        float db = Mathf.Log10(saved / 100f) * 20f;
        audioMixer.SetFloat("MasterVolume", db);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (instance == this) instance = null;
    }

    public void PausarMusica()
    {
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Pause();
    }

    public void ReanudarMusica()
    {
        if (audioSource != null && !audioSource.isPlaying)
            audioSource.UnPause();
    }
}
