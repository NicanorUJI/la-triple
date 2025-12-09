using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MapToggle : MonoBehaviour
{
    [Header("Refs")]
    public GameObject mapPanel;
    [Tooltip("Nombre del script de movimiento del Player (exacto).")]
    public string movementComponentName = "ArrowMovement";

    [Header("Map Settings")]
    [Tooltip("Escenas en las que el mapa puede abrirse.")]
    public string[] allowedScenes;

    [Header("Debug")]
    [SerializeField] private Behaviour playerMovementScript;
    private bool isOpen = false;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // Limpiar EventSystems duplicados (opcional)
        var eventSystems = FindObjectsByType<UnityEngine.EventSystems.EventSystem>(FindObjectsSortMode.None);
        for (int i = 1; i < eventSystems.Length; i++)
            Destroy(eventSystems[i].gameObject);
    }

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void Start()
    {
        if (mapPanel) mapPanel.SetActive(false);
        RebindPlayer();
    }

    private void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        RebindPlayer();
        if (mapPanel) mapPanel.SetActive(false);
        isOpen = false;
    }

    // ---------- Vincular Player por escena ----------
    private void RebindPlayer()
    {
        playerMovementScript = null;

        var player = GameObject.FindGameObjectWithTag("Player");
        if (!player) return;

        if (!string.IsNullOrWhiteSpace(movementComponentName))
        {
            var all = player.GetComponents<MonoBehaviour>();
            playerMovementScript = all.FirstOrDefault(c => c && c.GetType().Name == movementComponentName) as Behaviour;
        }

        if (!playerMovementScript)
        {
            var all = player.GetComponents<MonoBehaviour>();
            playerMovementScript = all.FirstOrDefault(c =>
                c && (c.GetType().Name.Contains("Move") || c.GetType().Name.Contains("Movement"))) as Behaviour;
        }
    }

    // ---------- Comprobar si esta escena permite abrir el mapa ----------
    private bool CanOpenMap()
    {
        string current = SceneManager.GetActiveScene().name;

        // Si no se configuró ninguna escena, permitir en todas
        if (allowedScenes == null || allowedScenes.Length == 0)
            return true;

        return allowedScenes.Contains(current);
    }

    // ---------- Input ----------
    void Update()
    {
        if (Keyboard.current?.cKey.wasPressedThisFrame == true) Toggle();
        if (isOpen && Keyboard.current?.escapeKey.wasPressedThisFrame == true) Close();
        if (Gamepad.current?.selectButton.wasPressedThisFrame == true) Toggle();
    }

    public void Toggle()
    {
        if (!mapPanel) return;

        // Bloqueo si la escena no permite abrir el mapa
        if (!CanOpenMap())
            return;

        isOpen = !isOpen;
        mapPanel.SetActive(isOpen);
        if (playerMovementScript) playerMovementScript.enabled = !isOpen;
    }

    public void Close()
    {
        if (!mapPanel || !isOpen) return;
        isOpen = false;
        mapPanel.SetActive(false);
        if (playerMovementScript) playerMovementScript.enabled = true;
    }
}
