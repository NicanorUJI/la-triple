using UnityEngine;
using UnityEngine.InputSystem; // SOLO New Input System

public class MapToggle : MonoBehaviour
{
    [Tooltip("Arrastra aquí el panel del mapa (MapPanel)")]
    public GameObject mapPanel;

    [Tooltip("Script que controla el movimiento del jugador")]
    public Behaviour playerMovementScript;

    bool isOpen;

    void Start()
    {
        if (mapPanel) mapPanel.SetActive(false);
    }

    void Update()
    {
        // Teclado: C para abrir/cerrar
        if (Keyboard.current?.cKey.wasPressedThisFrame == true)
            Toggle();

        // ESC para cerrar
        if (isOpen && Keyboard.current?.escapeKey.wasPressedThisFrame == true)
            Toggle();

        // Gamepad: botón Select/View
        if (Gamepad.current?.selectButton.wasPressedThisFrame == true)
            Toggle();
    }

    void Toggle()
    {
        if (!mapPanel) return;

        isOpen = !isOpen;
        mapPanel.SetActive(isOpen);

        if (playerMovementScript)
            playerMovementScript.enabled = !isOpen;
    }
}
