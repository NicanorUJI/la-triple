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

    // Corrutina: Solo se encarga de la transición visual y cargar la escena
    IEnumerator CambiarEscenaMapa(string escena)
    {
        if (fadeToBlack.Instance != null)
        {
            fadeToBlack.Instance.GetComponent<Animator>().SetTrigger("Start");
        }
        
        // Esperamos 1 segundo para que la pantalla se ponga en negro
        yield return new WaitForSeconds(1f);
        
        // Cargamos la escena
        SceneManager.LoadScene(escena);
    }

    // Este evento se dispara AUTOMÁTICAMENTE cuando la escena termina de cargar
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 1. Gestionar objetos visuales
        if (objetoAOcultar != null) objetoAOcultar.SetActive(false);
        if (objetoAActivar != null) objetoAActivar.SetActive(true);

        Debug.Log("[MAP] Escena cargada. Reanudando música inmediatamente.");

        // 2. Reanudar música directamente (sin espera)
        if (AudioManager.instance != null)
        {
            AudioManager.instance.ReanudarMusica();
        }
    }
}