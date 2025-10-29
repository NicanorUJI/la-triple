using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public string sceneToLoad;      // Escena destino
    public string entryPointName;   // Nombre del punto donde aparecerás en la nueva escena

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.lastExitName = entryPointName;
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
