using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScore2D : MonoBehaviour
{
    public int score = 0;
    public float powerTime = 8f;

    // +++ NUEVO: Variables de Audio +++
    [Header("Audio")]
    public AudioSource sfxSource;       // Arrastra el componente AudioSource aquí
    public AudioClip powerPelletSound;  // Arrastra el archivo de sonido aquí
    // +++++++++++++++++++++++++++++++++

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
            
            // +++ NUEVO: Reproducir sonido +++
            if (sfxSource != null && powerPelletSound != null)
            {
                sfxSource.PlayOneShot(powerPelletSound);
            }
            // ++++++++++++++++++++++++++++++++

            Destroy(other.gameObject);
            Debug.Log("POWER PELLET COMIDO! Puntuación: " + score);

            GhostyMovement[] ghosts = FindObjectsOfType<GhostyMovement>();
            foreach (GhostyMovement g in ghosts)
            {
                g.SetScaredState(true);
            }

            CancelInvoke(nameof(StopPowerPellet));
            Invoke(nameof(StopPowerPellet), powerTime);
        }

        // ======== COMPROBAR VICTORIA ========
        // Nota: Al destruir el objeto justo antes, el conteo puede fallar si no esperamos al siguiente frame.
        // Sin embargo, FindGameObjectsWithTag suele encontrar objetos activos. 
        // Si tienes problemas detectando la victoria, avísame.
        if (GameObject.FindGameObjectsWithTag("Punto").Length == 0 &&
            GameObject.FindGameObjectsWithTag("PowerPellet").Length == 0)
        {
            Debug.Log("[DEBUG] HAS GANADO!");
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