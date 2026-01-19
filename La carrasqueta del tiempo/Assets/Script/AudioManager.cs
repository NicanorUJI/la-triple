using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    
    [Header("Audio Mixer")]
    public AudioMixer mainMixer;
    public string musicParam = "MusicVolume";
    public string sfxParam = "SFXVolume";

    [Header("Clips de sonido UI/Efectos")]
    public AudioClip ClipbotonPausa;
    public AudioClip ClipbotonOpcion;
    public AudioClip ClipLlave;
    public AudioClip ClipPasos;
    public AudioClip ClipAgua;

    [Header("Música de Ambiente")]
    public AudioClip ClipMusicaAmbiente;
    private AudioSource musicaSource;

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
        musicaSource.outputAudioMixerGroup = GetMixerGroup("Musica"); 
        
        // --- CAMBIO AQUÍ ---
        // Establecemos el volumen inicial al 50% (0.5f)
        musicaSource.volume = 0.5f; 
        // -------------------

        if (ClipMusicaAmbiente != null)
        {
            musicaSource.clip = ClipMusicaAmbiente;
            musicaSource.Play();
        }

        // 2. Fuente para EFECTOS DE SONIDO (OneShot)
        efectoOneShotSource = gameObject.AddComponent<AudioSource>();
        efectoOneShotSource.playOnAwake = false;
        efectoOneShotSource.loop = false;
        efectoOneShotSource.outputAudioMixerGroup = GetMixerGroup("SFX"); 
    }

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
        newSource.outputAudioMixerGroup = GetMixerGroup("SFX");

        return newSource;
    }

    public void Reproducir(AudioClip clip)
    {
        if (clip != null && efectoOneShotSource != null)
        {
            efectoOneShotSource.PlayOneShot(clip, 1f); 
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
        // --- CAMBIO IMPORTANTE AQUÍ ---
        // Antes tenías puesto 1f. Si lo dejas en 1f, al despausar
        // volverá a sonar al 100%. Lo cambiamos a 0.5f también.
        if (musicaSource != null)
        {
            musicaSource.volume = 0.5f; 
            musicaSource.UnPause();
        }
        Debug.Log("Música reanudada");
    }
}