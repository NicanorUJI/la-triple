using UnityEngine;
using UnityEngine.SceneManagement;

public class MapUIController : MonoBehaviour
{
    [SerializeField] private MapToggle mapToggle;

    public void OnZoneClicked(string zoneName)
    {
        Debug.Log($"[MAP] Zona seleccionada: {zoneName}");

        if (mapToggle != null)
            mapToggle.Close();

        switch (zoneName.ToLower())
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
        }
    }
}
