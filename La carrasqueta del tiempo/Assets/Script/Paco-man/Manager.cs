using UnityEngine;

public class Manager : MonoBehaviour
{
    public static Manager Instance;

    [Header("Player")]
    public int playerLives = 3;
    public float invulnerabilityTime = 1f; // segundos de gracia tras recibir daño

    private bool playerInvulnerable = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
    }

    public void DamagePlayer()
    {
        if (playerInvulnerable) return;

        playerLives--;

        playerInvulnerable = true;
        Invoke(nameof(ResetInvulnerability), invulnerabilityTime);

        // Reinicia Pacman y fantasmas
        PacoManMovement player = FindObjectOfType<PacoManMovement>();
        if (player != null) player.RespawnPacman();

        GhostMovement[] ghosts = FindObjectsOfType<GhostMovement>();
        foreach (GhostMovement g in ghosts)
        {
            g.RespawnGhost();
        }
    }

    private void ResetInvulnerability()
    {
        playerInvulnerable = false;
    }
}
