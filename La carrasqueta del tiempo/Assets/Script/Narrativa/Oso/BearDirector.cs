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
    }

    private void SpawnBearNearPlayer()
    {
        // Si existe un punto de spawn en la escena, úsalo
        var spawn = GameObject.Find("SpawnOso");
        if (spawn != null)
        {
            bearInstance = Instantiate(bearPrefab, spawn.transform.position, Quaternion.identity);
            return;
        }

        // Si no, fallback: cerca del player
        var player = GameObject.FindGameObjectWithTag("Player");
        Vector3 pos = player != null
            ? player.transform.position + (Vector3)(Random.insideUnitCircle.normalized * 1.5f)
            : Vector3.zero;

        bearInstance = Instantiate(bearPrefab, pos, Quaternion.identity);
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
