using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SliderVolumen : MonoBehaviour
{
    [SerializeField] Slider soundSlider;
    [SerializeField] AudioMixer masterMixer;

    private const string VOLUME_KEY = "savedMasterVolume";

    private void Start()
    {
        float saved = PlayerPrefs.GetFloat(VOLUME_KEY, 100f);

        // Refrescar el slider antes de aplicar volumen
        soundSlider.value = saved;

        AplicarVolumen(saved);
    }

    public void CambiarVolumen(float value)
    {
        PlayerPrefs.SetFloat(VOLUME_KEY, value);
        AplicarVolumen(value);
    }

    private void AplicarVolumen(float value)
    {
        if (value < 1) value = 0.001f;

        float db = Mathf.Log10(value / 100f) * 20f;
        masterMixer.SetFloat("MasterVolume", db);
    }

    // Llamado por el slider
    public void SetVolumeFromSlider()
    {
        CambiarVolumen(soundSlider.value);
    }
}
