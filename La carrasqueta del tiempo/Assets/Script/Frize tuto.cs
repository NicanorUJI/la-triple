using UnityEngine;

public class PauseOnCanvasActive : MonoBehaviour
{
    void OnEnable()
    {
        // Congela el tiempo cuando el canvas se activa
        Time.timeScale = 0f;
    }

    void OnDisable()
    {
        // Reactiva el tiempo cuando el canvas se desactiva
        Time.timeScale = 1f;
    }
}
