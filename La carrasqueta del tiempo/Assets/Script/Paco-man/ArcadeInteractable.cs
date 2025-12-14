using UnityEngine;
using UnityEngine.SceneManagement;

public class ArcadeInteractable : MonoBehaviour, IInteractable
{
    [Header("Escena del minijuego")]
    public string pacmanSceneName = "inicio"; // pon aquí el nombre exacto de tu escena

    [Header("Opcional")]
    public bool onlyOnce = false;
    private bool used = false;

    public bool CanInteract()
    {
        if (onlyOnce && used) return false;
        return true;
    }

    public void Interact()
    {
        if (!CanInteract()) return;

        used = true;

        if (string.IsNullOrEmpty(pacmanSceneName))
        {
            Debug.LogError("[ArcadePacmanInteractable] pacmanSceneName está vacío.");
            return;
        }

        SceneManager.LoadScene(pacmanSceneName);
    }
}
