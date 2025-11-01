using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cargar escenas

public class SceneChangeFromPlayer : MonoBehaviour
{
    [SerializeField] private string sceneToLoad; // Nombre de la escena a cargar

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Portal"))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
