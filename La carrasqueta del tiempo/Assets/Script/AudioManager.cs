using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    
    [Header("Audio Mixer")]
    public AudioMixer mainMixer; // Referencia al Audio Mixer
    public string musicParam = "MusicVolume"; // Nombre del parámetro expuesto (MusicVolume)
    public string sfxParam = "SFXVolume"; // Nombre del parámetro expuesto (SFXVolume)

    [Header("Clips de sonido UI/Efectos")]
    public AudioClip ClipbotonPausa;
    public AudioClip ClipbotonOpcion;
    public AudioClip ClipLlave;
    public AudioClip ClipPasos;
    public AudioClip ClipAgua;

    [Header("Música de Ambiente")]
    public AudioClip ClipMusicaAmbiente;
    private AudioSource musicaSource; // Fuente dedicada a la música

    // Fuente para la reproducción puntual de efectos (PlayOneShot)
    private AudioSource efectoOneShotSource;

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

        // 1. Fuente para MÚSICA AMBIENTE
        musicaSource = gameObject.AddComponent<AudioSource>();
        musicaSource.playOnAwake = false;
        musicaSource.loop = true;
        // ¡ASIGNAMOS EL GRUPO DEL MIXER!
        musicaSource.outputAudioMixerGroup = GetMixerGroup("Musica"); 

        if (ClipMusicaAmbiente != null)
        {
            musicaSource.clip = ClipMusicaAmbiente;
            musicaSource.Play();
        }

        // 2. Fuente para EFECTOS DE SONIDO (OneShot)
        efectoOneShotSource = gameObject.AddComponent<AudioSource>();
        efectoOneShotSource.playOnAwake = false;
        efectoOneShotSource.loop = false;
        // ¡ASIGNAMOS EL GRUPO DEL MIXER!
        efectoOneShotSource.outputAudioMixerGroup = GetMixerGroup("SFX"); 
    }

    // Método de ayuda para obtener grupos del mixer
    private AudioMixerGroup GetMixerGroup(string groupName)
    {
        if (mainMixer == null) return null;

        AudioMixerGroup[] groups = mainMixer.FindMatchingGroups(groupName);
        if (groups.Length > 0)
        {
            return groups[0];
        }
        Debug.LogError($"Audio Mixer Group '{groupName}' no encontrado.");
        return null;
    }



    public AudioSource CrearAudioSourceEfecto(AudioClip clip, bool loop = false)
    {
        GameObject go = new GameObject($"Efecto_{clip.name}");
        go.transform.SetParent(this.transform);
        AudioSource newSource = go.AddComponent<AudioSource>();

        newSource.clip = clip;
        newSource.loop = loop;
        newSource.playOnAwake = false;
        newSource.outputAudioMixerGroup = GetMixerGroup("SFX"); // ¡ASIGNAMOS EL GRUPO SFX!

        // Nota: No necesitamos establecer newSource.volume aquí, ya que el volumen
        // final será controlado por el volumen del grupo SFX del Mixer.

        return newSource;
    }

    // Reproduce clips puntuales (botones, llaves, disparos, etc.)
    public void Reproducir(AudioClip clip)
    {
        if (clip != null && efectoOneShotSource != null)
        {
            efectoOneShotSource.PlayOneShot(clip, 1f); // Usamos volumen 1f, el mixer lo gestiona
        }
    }

    public void PausarMusica()
    {
        if (musicaSource != null)
            musicaSource.Pause();
        Debug.Log("Música pausada");
    }

    public void ReanudarMusica()
    {
        musicaSource.volume = 1f; // fuerza el volumen del AudioSource

        if (musicaSource != null)
            musicaSource.UnPause();
        Debug.Log("Música reanudada");

    }

}