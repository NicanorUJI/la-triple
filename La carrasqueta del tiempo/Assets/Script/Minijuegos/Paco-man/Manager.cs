using UnityEngine;
using UnityEngine.SceneManagement; // <- Para cambiar escenas

public class Manager : MonoBehaviour
{
    public static Manager Instance;
    [Header("Player")]
    public int playerLives = 3;
    public float invulnerabilityTime = 1f;
    private bool playerInvulnerable = false;
    [Header("UI")]
    public GameObject[] lifeIcons;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
    }

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
