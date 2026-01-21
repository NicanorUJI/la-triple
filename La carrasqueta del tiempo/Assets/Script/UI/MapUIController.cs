using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MapUIController : MonoBehaviour
{
    [SerializeField] private MapToggle mapToggle;

    [Header("Objetos a ocultar/activar al teletransportarse")]
    [SerializeField] private GameObject objetoAOcultar; 
    [SerializeField] private GameObject objetoAActivar;  

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnZoneClicked(string zoneName)
    {
        Debug.Log($"[MAP] Zona seleccionada: {zoneName}");

        if (mapToggle != null)
            mapToggle.Close();

        Time.timeScale = 1f; 

        // Aquí NO reanudamos la música todavía.

        switch (zoneName.ToLower())
        {
            case "carrasqueta": StartCoroutine(CambiarEscenaMapa("Carrasqueta")); break;
            case "plaza": StartCoroutine(CambiarEscenaMapa("Plaza")); break;
            case "placita": StartCoroutine(CambiarEscenaMapa("Placita")); break;
            case "esparde": StartCoroutine(CambiarEscenaMapa("espardeñes")); break;
            case "cementerio": StartCoroutine(CambiarEscenaMapa("Cementerio")); break;
            case "pozuelas": StartCoroutine(CambiarEscenaMapa("Pozuelas")); break;
            case "santa": StartCoroutine(CambiarEscenaMapa("SantaBarbara")); break;
            case "carrasquetap": StartCoroutine(CambiarEscenaMapa("CarrasquetaPasado")); break;
            case "plazap": StartCoroutine(CambiarEscenaMapa("PlazaPasado")); break;
            case "placitap": StartCoroutine(CambiarEscenaMapa("PlacitaPasado")); break;
            case "espardep": StartCoroutine(CambiarEscenaMapa("espardeñesPasado")); break;
            case "cementeriop": StartCoroutine(CambiarEscenaMapa("CementerioPasado")); break;
            case "pozuelasp": StartCoroutine(CambiarEscenaMapa("PozuelasPasado")); break;
            case "santap": StartCoroutine(CambiarEscenaMapa("SantaBarbaraPasado")); break;
            default:
                Debug.LogWarning($"[MAP] No hay escena configurada para: {zoneName}");
                break;
        }
    }

    // Corrutina 1: Solo se encarga de la transición visual y cargar la escena
    IEnumerator CambiarEscenaMapa(string escena)
    {
        if (fadeToBlack.Instance != null)
        {
            fadeToBlack.Instance.GetComponent<Animator>().SetTrigger("Start");
        }
        
        // Esperamos 1 segundo para que la pantalla se ponga en negro
        yield return new WaitForSeconds(1f);
        
        // Cargamos la escena (esto pausará el juego brevemente mientras carga)
        SceneManager.LoadScene(escena);
    }

    // Este evento se dispara AUTOMÁTICAMENTE cuando la escena termina de cargar
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 1. Gestionar objetos visuales (UI, HUD, etc.)
        if (objetoAOcultar != null) objetoAOcultar.SetActive(false);
        if (objetoAActivar != null) objetoAActivar.SetActive(true);

        Debug.Log("[MAP] Escena cargada. Iniciando espera para música...");

        // 2. Iniciamos la espera para la música en una nueva corrutina
        StartCoroutine(ReanudarMusicaConRetraso());
    }

    // Corrutina 2: Espera y activa el audio
    IEnumerator ReanudarMusicaConRetraso()
    {
        // Esperamos los 0.2 segundos que pediste DESPUÉS de cargar
        yield return new WaitForSeconds(0.2f);

        if (AudioManager.instance != null)
        {
            AudioManager.instance.ReanudarMusica();
            Debug.Log("[MAP] Música reanudada.");
        }
    }
}