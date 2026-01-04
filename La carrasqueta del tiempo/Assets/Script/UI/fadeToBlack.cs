using UnityEngine;

public class fadeToBlack : MonoBehaviour
{
    public static fadeToBlack Instance;
    public Animator anim;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // This keeps it alive!
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
