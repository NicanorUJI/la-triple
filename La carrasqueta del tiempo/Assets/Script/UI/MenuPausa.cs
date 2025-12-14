using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPausa : MonoBehaviour
{
    private const string SAVED_AMBIENTE_KEY = "SavedAmbienteVolume";
    private const string SAVED_SFX_KEY = "SavedSFXVolume";
    [SerializeField] private GameObject menuPausa;
    // subMenuSonido corresponde a SubMenuVolumen en la imagen
    [SerializeField] private GameObject subMenuSonido;
    [SerializeField] private GameObject botonPausa;
    // Necesitas una referencia para el botón que abre el submenú, si existe
    [SerializeField] private GameObject botonAbrirSonido; // ASUMIMOS que necesitas una referencia para el botón que lo abre.

    [SerializeField] private Slider sliderAmbiente;
    [SerializeField] private Slider sliderEfectos;
    [SerializeField] private AudioMixer masterMixer;

    
    // Almacenamos el AudioSource del sonido de agua para control de pausa/reanudar
    private AudioSource sonidoAguaSource;


    void Start()
    {

        // Asegúrate de que tanto el menú de pausa como el submenú de volumen estén ocultos al inicio.
        if (menuPausa != null)
            menuPausa.SetActive(false);

        if (subMenuSonido != null)
            subMenuSonido.SetActive(false);

        // Opcional: Cargar volumen inicial del Mixer a los Sliders (requiere PlayerPrefs o similar)
        // Por ahora, asumimos que el sliderAmbiente.value y sliderEfectos.value ya están en 1 (o en el valor deseado).
        CargarVolumenInicial();

        // **GESTIÓN DE SONIDO AGUA**
        if (AudioManager.instance.ClipAgua != null)
        {
            sonidoAguaSource = AudioManager.instance.CrearAudioSourceEfecto(AudioManager.instance.ClipAgua, true);
            sonidoAguaSource.Play();
        }

    }

    public void AbrirSubMenuVolumen()
    {
        if (AudioManager.instance.ClipbotonOpcion != null)
            AudioManager.instance.Reproducir(AudioManager.instance.ClipbotonOpcion);

        // 1. Ocultar el menú principal de pausa
        if (menuPausa != null)
            menuPausa.SetActive(false);

        // 2. Mostrar el submenú de volumen
        if (subMenuSonido != null)
            subMenuSonido.SetActive(true);
    }

    public void CerrarSubMenuVolumen()
    {
        if (AudioManager.instance.ClipbotonOpcion != null)
            AudioManager.instance.Reproducir(AudioManager.instance.ClipbotonOpcion);

        if (subMenuSonido != null)
            subMenuSonido.SetActive(false);

        if (menuPausa != null)
            menuPausa.SetActive(true);
    }

    // -------------------------------------

    public void Pausa()
    {
        if (AudioManager.instance.ClipbotonPausa != null)
            AudioManager.instance.Reproducir(AudioManager.instance.ClipbotonPausa);

        Time.timeScale = 1f;

        // Ocultar el botón de pausa en pantalla
        if (botonPausa != null)
            botonPausa.SetActive(false);

        // Mostrar el menú de pausa
        if (menuPausa != null)
            menuPausa.SetActive(true);
        
        // Asegurarse de que el submenú de volumen esté oculto al entrar en la pausa
        if (subMenuSonido != null)
            subMenuSonido.SetActive(false); 

        // PAUSAR AUDIO:
        AudioManager.instance.PausarMusica(); 

        if (sonidoAguaSource != null)
            sonidoAguaSource.Pause(); 
    }
    
    void OnDestroy()
    {
        if (sonidoAguaSource != null)
        {
            Destroy(sonidoAguaSource.gameObject);
        }
    }

    public void Reanudar()
    {

        Time.timeScale = 1f;

        if (botonPausa != null)
            botonPausa.SetActive(true);
            
        if (menuPausa != null)
            menuPausa.SetActive(false);

        // Asegurarse de ocultar el submenú por si acaso
        if (subMenuSonido != null)
            subMenuSonido.SetActive(false);

        // REANUDAR AUDIO:
        AudioManager.instance.ReanudarMusica(); 

        if (AudioManager.instance.ClipbotonOpcion != null)
            AudioManager.instance.Reproducir(AudioManager.instance.ClipbotonOpcion);

        if (sonidoAguaSource != null)
            sonidoAguaSource.UnPause(); 
    }

private void CargarVolumenInicial()
    {
        // CARGA: Usamos las claves que usamos para GUARDAR y el valor por defecto 100 (como en tu script de ejemplo)
        float ambienteValue = PlayerPrefs.GetFloat(SAVED_AMBIENTE_KEY, 100f);
        float efectosValue = PlayerPrefs.GetFloat(SAVED_SFX_KEY, 100f);

        // 1. Asignar los valores guardados a los Sliders (rango 0-100)
        if (sliderAmbiente != null)
        {
            sliderAmbiente.value = ambienteValue;
        }
        if (sliderEfectos != null)
        {
            sliderEfectos.value = efectosValue;
        }

        // 2. Aplicar los valores cargados al Audio Mixer inmediatamente
        // ESTO REEMPLAZA LA FUNCIÓN RefreshSlider, ya que establece el valor del mixer.
        CambiarVolumenAmbiente(ambienteValue);
        CambiarVolumenEfectos(efectosValue);
    }

    public void CambiarVolumenAmbiente(float v)
    {
        float volumenDb;

        // Si el slider está en 0, aplica silencio total.
        if (v <= 0f) 
        {
            volumenDb = -80f; 
        }
        else
        {
            // La fórmula de conversión logarítmica es correcta para 0-100:
            // (Log10 de [valor entre 0 y 1]) * 20
            volumenDb = Mathf.Log10(v / 100f) * 20f;
        }

        // Aplica el valor dB al parámetro expuesto (MusicVolume)
        if (masterMixer != null)
        {
            masterMixer.SetFloat("MusicVolume", volumenDb);
        }

        // GUARDA: Usa la clave correcta.
        PlayerPrefs.SetFloat(SAVED_AMBIENTE_KEY, v);
        
        AudioManager.instance.Reproducir(AudioManager.instance.ClipbotonOpcion);
    }

    public void CambiarVolumenEfectos(float v)
    {
        float volumenDb;

        if (v <= 0f) 
        {
            volumenDb = -80f; 
        }
        else
        {
            // Fórmula Logarítmica para SFX
            volumenDb = Mathf.Log10(v / 100f) * 20f;
        }

        // Aplica el valor dB al parámetro expuesto (SFXVolume)
        if (masterMixer != null)
        {
            masterMixer.SetFloat("SFXVolume", volumenDb);
        }

        // GUARDA: Usa la clave correcta.
        PlayerPrefs.SetFloat(SAVED_SFX_KEY, v);
        
        AudioManager.instance.Reproducir(AudioManager.instance.ClipbotonOpcion);
    }

    public void Cerrar()
    {
        AudioManager.instance.Reproducir(AudioManager.instance.ClipbotonOpcion);
        Debug.Log("Cerrando");

        Time.timeScale = 1f;
        Destroy(AudioManager.instance.gameObject); 

        // Aseguramos que se guarde cualquier cambio antes de cargar la siguiente escena
        PlayerPrefs.Save();

        SceneManager.LoadScene("Escena Menú");
    }

    public void GuardarPartida()
    {
        AudioManager.instance.Reproducir(AudioManager.instance.ClipbotonOpcion);
        Debug.Log("Partida guardada.");
    }

}