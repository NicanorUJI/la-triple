using UnityEngine;
using UnityEngine.SceneManagement;

public class Viaje : MonoBehaviour, IInteractable
{
    [SerializeField] private string sceneToLoad; // Nombre de la escena a cargar

    public bool CanInteract()
    {
        return true; // Siempre se puede interactuar
    }

    public void Interact()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.Log("Cambiando de escena a: " + sceneToLoad);
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("No se ha asignado una escena en el NPC.");
        }
    }
}
