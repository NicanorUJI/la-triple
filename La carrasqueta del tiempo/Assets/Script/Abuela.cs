using UnityEngine;

public class DontDestroyThis : MonoBehaviour
{
    private static DontDestroyThis instance;

    void Awake()
    {
        // Evita duplicados
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
