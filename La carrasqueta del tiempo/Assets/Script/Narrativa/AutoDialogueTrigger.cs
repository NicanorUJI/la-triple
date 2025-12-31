using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AutoDialogueTrigger : MonoBehaviour
{
    public NPC npc;                     // NPC al que le vamos a disparar el diálogo
    public bool oneShot = true;         // Solo la primera vez
    public string[] requiredFlags;      // Flags que deben estar activos
    public string[] forbiddenFlags;     // Flags que NO deben estar activos

    private bool alreadyTriggered = false;

    private void Reset()
    {
        // Si el trigger está en el mismo GameObject que el NPC, lo cogemos automáticamente
        if (npc == null)
            npc = GetComponent<NPC>();

        // Aseguramos isTrigger = true
        var col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (oneShot && alreadyTriggered)
            return;

        if (!CheckFlags())
            return;

        alreadyTriggered = true;

        if (npc != null)
        {
            npc.Interact();   // Esto inicia el diálogo igual que si pulsaras espacio
        }
        else
        {
            Debug.LogWarning($"AutoDialogueTrigger en {name} no tiene NPC asignado.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!alreadyTriggered)
        {
            if (!other.CompareTag("Player"))
                return;

            if (oneShot && alreadyTriggered)
                return;

            if (!CheckFlags())
                return;

            alreadyTriggered = true;

            if (npc != null)
            {
                npc.Interact();   // Esto inicia el diálogo igual que si pulsaras espacio
            }
            else
            {
                Debug.LogWarning($"AutoDialogueTrigger en {name} no tiene NPC asignado.");
            }
        }
        
    }

    private bool CheckFlags()
    {
        // Requisitos
        if (requiredFlags != null)
        {
            foreach (var flag in requiredFlags)
            {
                if (!string.IsNullOrEmpty(flag) && !GameManager.Check(flag))
                    return false;
            }
        }

        // Prohibidos
        if (forbiddenFlags != null)
        {
            foreach (var flag in forbiddenFlags)
            {
                if (!string.IsNullOrEmpty(flag) && GameManager.Check(flag))
                    return false;
            }
        }

        return true;
    }
}
