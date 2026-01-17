using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MapUIController : MonoBehaviour
{
    [SerializeField] private MapToggle mapToggle;

    [Header("Objetos a ocultar/activar al teletransportarse")]
    [SerializeField] private GameObject objetoAOcultar; // ej: panel del mapa
    [SerializeField] private GameObject objetoAActivar;  // ej: HUD

    void Awake()
    {
        // Opcional: que persista entre escenas si quieres
        DontDestroyOnLoad(gameObject);

        // Suscribirse al evento de cambio de escena
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

        Time.timeScale = 1f; // reanuda el juego por si estaba en pausa

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

    IEnumerator CambiarEscenaMapa(string escena)
    {
        if (fadeToBlack.Instance != null)
        {
            fadeToBlack.Instance.GetComponent<Animator>().SetTrigger("Start");
        }
        else
        {
            Debug.LogWarning("No SceneTransition instance found in scene!");
        }

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(escena);
    }

    // ===================== ACTIVAR/DESACTIVAR OBJETOS =====================
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Desactiva el objeto (ej: panel del mapa)
        if (objetoAOcultar != null)
            objetoAOcultar.SetActive(false);

        // Activa el objeto (ej: HUD)
        if (objetoAActivar != null)
            objetoAActivar.SetActive(true);

        Debug.Log("[MAP] Objetos activados/desactivados tras teletransporte");
    }
}
