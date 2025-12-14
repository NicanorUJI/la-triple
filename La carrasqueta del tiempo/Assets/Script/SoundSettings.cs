using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundSettings : MonoBehaviour
{
    [SerializeField] private Slider soundSlider;
    [SerializeField] private AudioMixer masterMixer;

    private void Start()
    {
        SetVolume(PlayerPrefs.GetFloat("SavedMasterVolume", 100));
    }

    public void SetVolume(float value)
    {
        if(value<1){ value = .001f;}
        RefreshSlider(value);
        PlayerPrefs.SetFloat("SavedMasterVolume",value);
        masterMixer.SetFloat("MusicVolume",Mathf.Log10(value/100)*20f);
    }

    public void SetVolumeFromSlider()
    {
        SetVolume(soundSlider.value);
    }

    public void RefreshSlider(float value)
    {
        soundSlider.value = value;
    }
}
