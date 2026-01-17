using UnityEngine;
using UnityEngine.SceneManagement;

public class SalirJuego : MonoBehaviour
{
    // Este método se llamará al presionar el botón
    public void Salir()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }

    public void AbrirCreditos()
    {
        // Cargar la escena "Creditos"
        SceneManager.LoadScene("Creditos");
    }
}
