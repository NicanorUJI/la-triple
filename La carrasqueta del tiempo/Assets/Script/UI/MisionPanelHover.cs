using UnityEngine;
using TMPro;

public class MissionPanelHover : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private RectTransform panel;      // El propio MissionPanel
    [SerializeField] private TMP_Text missionText;     // Texto de la misión (opcional)

    [Header("Zona de apertura (0-1)")]
    [Tooltip("Porcentaje de la pantalla a partir del cual se abre el panel (ej: 0.97 = 97% del ancho).")]
    [Range(0.8f, 1f)]
    [SerializeField] private float openThreshold = 0.97f;

    [Tooltip("Porcentaje de la pantalla por debajo del cual se cierra el panel (ligeramente menor que openThreshold).")]
    [Range(0.8f, 1f)]
    [SerializeField] private float closeThreshold = 0.95f;

    [Header("Animación")]
    [SerializeField] private float slideSpeed = 8f;

    private Vector2 closedPos;
    private Vector2 openPos;
    private bool isOpen;

    private void Reset()
    {
        panel = GetComponent<RectTransform>();
        missionText = GetComponentInChildren<TMP_Text>();
    }

    private void Awake()
    {
        if (panel == null)
            panel = GetComponent<RectTransform>();

        // Posición inicial del panel en la escena = posición cerrada (fuera de pantalla)
        closedPos = panel.anchoredPosition;
        // Posición abierta = misma Y, desplazada hacia la izquierda lo que mide el panel
        openPos = closedPos + new Vector2(-panel.rect.width, 0f);
    }

    private void Update()
    {
        // Normalizamos la posición X del ratón a [0,1]
        float mouseXRatio = 0f;
        if (Screen.width > 0)
            mouseXRatio = Input.mousePosition.x / Screen.width;

        // Lógica de apertura / cierre según la posición del ratón
        if (!isOpen && mouseXRatio >= openThreshold)
        {
            isOpen = true;
        }
        else if (isOpen && mouseXRatio <= closeThreshold)
        {
            isOpen = false;
        }

        // Animar suavemente la posición del panel hacia su destino
        Vector2 target = isOpen ? openPos : closedPos;
        panel.anchoredPosition = Vector2.Lerp(
            panel.anchoredPosition,
            target,
            Time.unscaledDeltaTime * slideSpeed
        );
    }

    // Por si luego querés cambiar el texto desde otro script
    public void SetMissionText(string text)
    {
        if (missionText != null)
            missionText.text = text;
    }
}
