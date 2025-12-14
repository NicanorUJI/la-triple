using UnityEngine;
using UnityEngine.SceneManagement;

public class CambiarEscena : MonoBehaviour
{
    // Esta función la podrás asignar al botón desde el Inspector
    public void Play()
    {
        SceneManager.LoadScene("Arcade");
    }

    public void Exit()
    {
        SceneManager.LoadScene("Bar");
    }
}