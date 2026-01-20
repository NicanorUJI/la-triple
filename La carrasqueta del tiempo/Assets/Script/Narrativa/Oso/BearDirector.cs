using UnityEngine;
using UnityEngine.SceneManagement;

public class BearDirector : MonoBehaviour
{
    public static BearDirector Instance;

    [Header("Prefab del oso (con BearFollower2D + Rigidbody2D + Collider2D)")]
    public GameObject bearPrefab;

    private GameObject bearInstance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        // Nunca en menú
        if (SceneManager.GetActiveScene().name == "Escena Menú")
        {
            DestroyBear();
            return;
        }

        // Si Acto 3 terminó, destruir
        if (GameManager.Check("Act3_End"))
        {
            DestroyBear();
            return;
        }

        // Si el oso no está activo, destruir
        if (!GameManager.Check("Act3_BearActive"))
        {
            DestroyBear();
            return;
        }

        // Si está activo y no existe, spawnear
        if (bearInstance == null && bearPrefab != null)
        {
            SpawnBearNearPlayer();
        }

        if (GameManager.Check("Act3_PastCelebrationDone"))
        {
            bearInstance.SetActive(false);
        }
        if (GameManager.Check("Act3_CarrasquetaPresentWarned"))
        {
            bearInstance.SetActive(true);
        }
    }

    private void SpawnBearNearPlayer()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        bearInstance = Instantiate(bearPrefab);

        var follower = bearInstance.GetComponent<BearFollower2D>();
        if (follower != null)
        {
            string sceneName = SceneManager.GetActiveScene().name;

            // Escenas donde el oso va a la derecha
            if (sceneName == "Carrasqueta" || sceneName == "CarrasquetaPasado")
            {
                follower.followOffsetX = Mathf.Abs(follower.followOffsetX);
            }
            else
            {
                // Por seguridad, forzamos izquierda en el resto
                follower.followOffsetX = -Mathf.Abs(follower.followOffsetX);
            }

            follower.InitAtPlayer(player.transform);
        }
    }

    private void DestroyBear()
    {
        if (bearInstance != null)
        {
            Destroy(bearInstance);
            bearInstance = null;
        }
    }
}
