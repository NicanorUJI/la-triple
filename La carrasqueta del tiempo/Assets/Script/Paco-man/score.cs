using UnityEngine;
using UnityEngine.SceneManagement; // <- Para cambiar escenas

public class PlayerScore2D : MonoBehaviour
{
    public int score = 0;
    public float powerTime = 8f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Punto"))
        {
            score += 10;
            Destroy(other.gameObject);
            Debug.Log("Puntuación: " + score);
        }
        if (other.CompareTag("PowerPellet"))
        {
            score += 100;
            Destroy(other.gameObject);
            Debug.Log("¡POWER PELLET COMIDO! Puntuación: " + score);

            GhostyMovement[] ghosts = FindObjectsOfType<GhostyMovement>();
            foreach (GhostyMovement g in ghosts)
            {
                g.SetScaredState(true);
            }

            CancelInvoke(nameof(StopPowerPellet));
            Invoke(nameof(StopPowerPellet), powerTime);
        }

        // ======== COMPROBAR VICTORIA ========
        if (GameObject.FindGameObjectsWithTag("Punto").Length == 0 &&
            GameObject.FindGameObjectsWithTag("PowerPellet").Length == 0)
        {
            Debug.Log("[DEBUG] ¡HAS GANADO!");
            SceneManager.LoadScene("Win");
        }
    }

    private void StopPowerPellet()
    {
        GhostyMovement[] ghosts = FindObjectsOfType<GhostyMovement>();
        foreach (GhostyMovement g in ghosts)
            g.SetScaredState(false);
    }
}
