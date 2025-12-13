using UnityEngine;
using UnityEngine.SceneManagement;
public class PuertaAbiertaTP : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "Plaza";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Entrando por la puerta abierta");
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
