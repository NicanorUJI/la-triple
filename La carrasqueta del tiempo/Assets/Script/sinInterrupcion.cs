using UnityEngine;

public class MusicaAmbiente : MonoBehaviour
{
    public static MusicaAmbiente instance;

    public AudioSource audioSource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Si no está asignado en el inspector, lo obtenemos
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void PausarMusica()
    {
        if (audioSource != null)
            audioSource.Pause();
    }

    public void ReanudarMusica()
    {
        if (audioSource != null)
            audioSource.UnPause();
    }
}
