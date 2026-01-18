using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MenuPausa : MonoBehaviour
{
    public static MenuPausa Instance;

    private const string SAVED_AMBIENTE_KEY = "SavedAmbienteVolume";
    private const string SAVED_SFX_KEY = "SavedSFXVolume";
    public bool botonDesbloqueado = false;

    [SerializeField] private GameObject menuPausa;
    [SerializeField] private GameObject subMenuSonido;
    [SerializeField] private GameObject botonPausa;

    [Header("Scroll View")]
    [SerializeField] private GameObject scrollView;

    [SerializeField] private Slider sliderAmbiente;
    [SerializeField] private Slider sliderEfectos;
    [SerializeField] private AudioMixer masterMixer;

    private AudioSource sonidoAguaSource;

    [Header("Escenas sin menú")]
    [SerializeField] private List<string> escenasSinMenu = new List<string>(); // Lista de escenas donde ocultar menú

    [Header("Panel de misiones")]
    [SerializeField] private GameObject missionPanel;

    private bool iniciado = false; // Para que Start solo corra una vez

    void Awake()
    {
        // ------------------ SINGLETON ------------------
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persistir entre escenas
        // ------------------------------------------------

        // Suscribirse al evento de cambio de escena
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        if (iniciado) return;
        iniciado = true;

        menuPausa?.SetActive(false);
        subMenuSonido?.SetActive(false);
        scrollView?.SetActive(false);

        CargarVolumenInicial();

        if (AudioManager.instance.ClipAgua != null)
        {
            sonidoAguaSource = AudioManager.instance.CrearAudioSourceEfecto(
                AudioManager.instance.ClipAgua, true);
            sonidoAguaSource.Play();
        }
    }

    // ================= ESCENA =================
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(AjustarUI(scene));
    }

    private IEnumerator AjustarUI(Scene scene)
    {
        yield return null; // esperar un frame para que otros scripts terminen de activar/desactivar
        bool ocultarMenu = escenasSinMenu.Exists(s => s.Trim().ToLower() == scene.name.Trim().ToLower());
        Debug.Log($"Escena cargada: {scene.name} | Ocultar menú: {ocultarMenu}");


        subMenuSonido?.SetActive(false);
        if (botonDesbloqueado)
        {
            botonPausa?.SetActive(!ocultarMenu);
        }
        else
        {
            botonPausa?.SetActive(false);
        }
        scrollView?.SetActive(false);
        missionPanel?.SetActive(!ocultarMenu);
    }


    // ================= SCROLL =================
    public void ToggleScroll()
    {
        if (scrollView == null) return;

        bool nuevoEstado = !scrollView.activeSelf;
        OcultarScroll();
        scrollView.SetActive(nuevoEstado);
    }

    public void OcultarScroll()
    {
        if (scrollView != null)
            scrollView.SetActive(false);
    }

    // ================= MENÚ =================
    public void Pausa()
    {
        AudioManager.instance.Reproducir(AudioManager.instance.ClipbotonPausa);

        Time.timeScale = 0f;
        botonPausa?.SetActive(false);
        menuPausa?.SetActive(true);
        subMenuSonido?.SetActive(false);
        OcultarScroll();

        AudioManager.instance.PausarMusica();
        sonidoAguaSource?.Pause();
    }

    public void Reanudar()
    {
        Time.timeScale = 1f;
        botonPausa?.SetActive(true);
        menuPausa?.SetActive(false);
        subMenuSonido?.SetActive(false);
        OcultarScroll();

        AudioManager.instance.ReanudarMusica();
        AudioManager.instance.Reproducir(AudioManager.instance.ClipbotonOpcion);
        sonidoAguaSource?.UnPause();
    }

    public void AbrirSubMenuVolumen()
    {
        Debug.Log(gameObject.name);
        Debug.Log("Hola");
        AudioManager.instance.Reproducir(AudioManager.instance.ClipbotonOpcion);

        menuPausa?.SetActive(false);
        subMenuSonido?.SetActive(true);
        OcultarScroll();
    }

    public void CerrarSubMenuVolumen()
    {
        AudioManager.instance.Reproducir(AudioManager.instance.ClipbotonOpcion);

        subMenuSonido?.SetActive(false);
        menuPausa?.SetActive(true);
        OcultarScroll();
    }

    // ================= AUDIO =================
    private void CargarVolumenInicial()
    {
        float ambiente = PlayerPrefs.GetFloat(SAVED_AMBIENTE_KEY, 100f);
        float efectos = PlayerPrefs.GetFloat(SAVED_SFX_KEY, 100f);

        sliderAmbiente.value = ambiente;
        sliderEfectos.value = efectos;

        CambiarVolumenAmbiente(ambiente);
        CambiarVolumenEfectos(efectos);
    }

    public void CambiarVolumenAmbiente(float v)
    {
        float db = v <= 0 ? -80f : Mathf.Log10(v / 100f) * 20f;
        masterMixer.SetFloat("MusicVolume", db);
        PlayerPrefs.SetFloat(SAVED_AMBIENTE_KEY, v);
        AudioManager.instance.Reproducir(AudioManager.instance.ClipbotonOpcion);
    }

    public void CambiarVolumenEfectos(float v)
    {
        float db = v <= 0 ? -80f : Mathf.Log10(v / 100f) * 20f;
        masterMixer.SetFloat("SFXVolume", db);
        PlayerPrefs.SetFloat(SAVED_SFX_KEY, v);
        AudioManager.instance.Reproducir(AudioManager.instance.ClipbotonOpcion);
    }

    // ================= OTROS =================
    public void Cerrar()
    {
        AudioManager.instance.Reproducir(AudioManager.instance.ClipbotonOpcion);
        Time.timeScale = 1f;
        botonPausa?.SetActive(true);
        menuPausa?.SetActive(false);
        subMenuSonido?.SetActive(false);
        OcultarScroll();
        PlayerPrefs.Save();
        Destroy(AudioManager.instance.gameObject);
        SceneManager.LoadScene("Escena Menú");
    }

    public void GuardarPartida()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("No se encontró el jugador.");
            return;
        }

        GameData data = new GameData();

        // Posición
        data.playerX = player.transform.position.x;
        data.playerY = player.transform.position.y;
        data.playerZ = player.transform.position.z;
        data.sceneName = SceneManager.GetActiveScene().name;

        // Volumen
        data.volumenAmbiente = sliderAmbiente.value;
        data.volumenEfectos = sliderEfectos.value;

        // 🔴 GUARDAR FLAGS (REWARDS)
        if (RewardManager.Instance != null)
            data.rewards = new List<string>(RewardManager.Instance.rewards);

        SaveSystem.Save(data);
        Debug.Log("✅ Partida guardada con rewards");
    }
}
