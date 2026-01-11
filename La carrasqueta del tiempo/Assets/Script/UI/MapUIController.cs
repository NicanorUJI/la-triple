using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MapUIController : MonoBehaviour
{
    [SerializeField] private MapToggle mapToggle;
   

    public void OnZoneClicked(string zoneName)
    {
        Debug.Log($"[MAP] Zona seleccionada: {zoneName}");

        if (mapToggle != null)
            mapToggle.Close();

        /*switch (zoneName.ToLower())
        {
            case "carrasqueta": SceneManager.LoadScene("Carrasqueta"); break;
            case "plaza":       SceneManager.LoadScene("Plaza");       break;
            case "placita":      SceneManager.LoadScene("Placita");      break;
            case "esparde": SceneManager.LoadScene("espardeñes"); break;
            case "cementerio": SceneManager.LoadScene("Cementerio"); break;
            case "pozuelas": SceneManager.LoadScene("Pozuelas"); break;
            case "santa": SceneManager.LoadScene("SantaBarbara"); break;
            case "carrasquetap": SceneManager.LoadScene("CarrasquetaPasado"); break;
            case "plazap": SceneManager.LoadScene("PlazaPasado"); break;
            case "placitap": SceneManager.LoadScene("PlacitaPasado"); break;
            case "espardep": SceneManager.LoadScene("espardeñesPasado"); break;
            case "cementeriop": SceneManager.LoadScene("CementerioPasado"); break;
            case "pozuelasp": SceneManager.LoadScene("PozuelasPasado"); break;
            case "santap": SceneManager.LoadScene("SantaBarbaraPasado"); break;
            default:
                Debug.LogWarning($"[MAP] No hay escena configurada para: {zoneName}");
                break;
        }*/
        
        switch (zoneName.ToLower())
        {
            case "carrasqueta": StartCoroutine(CambiarEscenaMapa("Carrasqueta")); break;
            case "plaza":       StartCoroutine(CambiarEscenaMapa("Plaza")); break;
            case "placita":     StartCoroutine(CambiarEscenaMapa("Placita")); break;
            case "esparde":     StartCoroutine(CambiarEscenaMapa("espardeñes")); break;
            case "cementerio":  StartCoroutine(CambiarEscenaMapa("Cementerio")); break;
            case "pozuelas":    StartCoroutine(CambiarEscenaMapa("Pozuelas")); break;
            case "santa":       StartCoroutine(CambiarEscenaMapa("SantaBarbara")); break;
            case "carrasquetap": StartCoroutine(CambiarEscenaMapa("CarrasquetaPasado")); break;
            case "plazap":      StartCoroutine(CambiarEscenaMapa("PlazaPasado")); break;
            case "placitap":    StartCoroutine(CambiarEscenaMapa("PlacitaPasado")); break;
            case "espardep":    StartCoroutine(CambiarEscenaMapa("espardeñesPasado")); break;
            case "cementeriop": StartCoroutine(CambiarEscenaMapa("CementerioPasado")); break;
            case "pozuelasp":   StartCoroutine(CambiarEscenaMapa("PozuelasPasado")); break;
            case "santap":      StartCoroutine(CambiarEscenaMapa("SantaBarbaraPasado")); break;
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

        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(escena);
    }

}
