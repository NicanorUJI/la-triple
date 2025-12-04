using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenu : MonoBehaviour
{
    [Header("Nombre de la escena del menú principal")]
    public string menuSceneName = "MainMenu"; // Cambia esto por el nombre real de tu escena de inicio

    // Método que se ejecutará al presionar el botón
    public void GoToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}

