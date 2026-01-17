using UnityEngine;
using UnityEngine.InputSystem;

public class CutsceneDetector : MonoBehaviour
{
    private IInteractable interactableInRange = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current?.spaceKey.wasPressedThisFrame == true)
        {
            // Si no hay nada cerca, no hagas nada (evita el NullReference)
            if (interactableInRange == null)
                return;


            interactableInRange.Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
        {
            interactableInRange = interactable;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable == interactableInRange)
        {
            interactableInRange = null;
        }
    }

}


