using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButtonUI : MonoBehaviour
{
    public void PlayGame()
    {
        // Cambia "GameScene" por el nombre real de tu escena
        SceneManager.LoadScene("Plaza");
    }
}
