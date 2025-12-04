using UnityEngine;

public class PlayerScore2D : MonoBehaviour
{
    public int score = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Punto"))
        {
            score= score + 10;
            Destroy(other.gameObject);
            Debug.Log("Puntuación: " + score);
        }
    }
}
