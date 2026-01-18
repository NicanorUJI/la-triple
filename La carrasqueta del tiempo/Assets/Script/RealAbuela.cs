using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentCharacter : MonoBehaviour
{
    private static PersistentCharacter instance;

    [SerializeField] private GameObject characterVisual;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Plaza")
        {
            characterVisual.SetActive(true);
        }
        else
        {
            characterVisual.SetActive(false);
        }
    }
}
