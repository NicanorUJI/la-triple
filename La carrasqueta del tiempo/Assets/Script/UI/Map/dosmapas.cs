using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    public MapToggle prefabMapA;
    public MapToggle prefabMapB;

    private MapToggle mapA;
    private MapToggle mapB;

    private static MapManager _instance;

    void Awake()
    {
        // Singleton del MapManager
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        // Instanciar mapa A si no existe
        if (mapA == null && prefabMapA != null)
        {
            mapA = Instantiate(prefabMapA);
            mapA.name = "MapaA";
            DontDestroyOnLoad(mapA.gameObject);
        }

        // Instanciar mapa B si no existe
        if (mapB == null && prefabMapB != null)
        {
            mapB = Instantiate(prefabMapB);
            mapB.name = "MapaB";
            DontDestroyOnLoad(mapB.gameObject);
        }
    }

    public void OpenMapForCurrentScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        // Cerrar todos los mapas abiertos
        if (mapA != null && mapA.isOpen) mapA.Close();
        if (mapB != null && mapB.isOpen) mapB.Close();

        // Abrir solo el mapa que tenga esta escena en allowedScenes
        if (mapA != null && mapA.allowedScenes != null &&
            System.Array.Exists(mapA.allowedScenes, s => s == currentScene.name))
        {
            mapA.Toggle();
            return;
        }

        if (mapB != null && mapB.allowedScenes != null &&
            System.Array.Exists(mapB.allowedScenes, s => s == currentScene.name))
        {
            mapB.Toggle();
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Activar solo los mapas que tengan esta escena en allowedScenes
        if (mapA != null)
            mapA.gameObject.SetActive(mapA.allowedScenes != null && System.Array.Exists(mapA.allowedScenes, s => s == scene.name));

        if (mapB != null)
            mapB.gameObject.SetActive(mapB.allowedScenes != null && System.Array.Exists(mapB.allowedScenes, s => s == scene.name));
    }
}
