using UnityEngine;
using UnityEngine.SceneManagement; 

public class Manager : MonoBehaviour
{
    public static Manager Instance;
    
    [Header("Player")]
    public int playerLives = 3;
    public float invulnerabilityTime = 1f;
    private bool playerInvulnerable = false;
    
    [Header("UI")]
    public GameObject[] lifeIcons;

    // +++ NUEVO: Variables de Audio +++
    [Header("Audio")]
    public AudioSource musicSource;       // Arrastra aquí el componente AudioSource
    public AudioClip backgroundMusic;     // Arrastra aquí tu canción
    // +++++++++++++++++++++++++++++++++

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
    }

    // +++ NUEVO: Start para iniciar la música +++
    private void Start()
    {
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true; // Asegura que la música se repita
            musicSource.Play();
        }
    }
    // +++++++++++++++++++++++++++++++++++++++++++

    public void DamagePlayer()
    {
        if (playerInvulnerable) return;

        playerLives--;

        if (lifeIcons != null && playerLives >= 0 && playerLives < lifeIcons.Length)
        {
            if (lifeIcons[playerLives] != null)
                lifeIcons[playerLives].SetActive(false);
        }

        // ======== COMPROBAR GAME OVER ========
        if (playerLives <= 0)
        {
            Debug.Log("[DEBUG MANAGER] GAME OVER");
            
            // Al cambiar de escena, este objeto se destruye y la música parará sola
            SceneManager.LoadScene("Game-Over");
            return;
        }

        PacoManMovement player = FindObjectOfType<PacoManMovement>();
        if (player != null) player.RespawnPacman();

        GhostyMovement[] ghosts = FindObjectsOfType<GhostyMovement>();
        foreach (GhostyMovement g in ghosts)
        {
            g.RespawnGhost();
        }

        playerInvulnerable = true;
        Invoke(nameof(ResetInvulnerability), invulnerabilityTime);
    }

    private void ResetInvulnerability()
    {
        playerInvulnerable = false;
    }
}