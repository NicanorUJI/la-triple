using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayButtonUI : MonoBehaviour
{
    public void PlayGame()
    {

        SaveSystem.DeleteSave();

        // Cambia "GameScene" por el nombre real de tu escena
        SceneManager.LoadScene("Plaza");
    }
}
