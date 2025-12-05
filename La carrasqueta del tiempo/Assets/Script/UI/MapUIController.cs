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
            default:
                Debug.LogWarning($"[MAP] No hay escena configurada para: {zoneName}");
                break;
        }
    }
}
