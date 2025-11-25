using UnityEngine;

public class Puerta1Interactable : MonoBehaviour, IInteractable
{
    public bool CanInteract()
    {
        return true; // Siempre interactuable
    }

    public void Interact()
    {
        // No haces nada aquí porque InteractionIndoor gestiona la lógica
        // Solo se necesita para que InteractionIndoor lo detecte.
    }
}