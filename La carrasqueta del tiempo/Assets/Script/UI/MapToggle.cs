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

    [Header("Debug")]
    [SerializeField] private Behaviour playerMovementScript;
    private bool isOpen = false;

    // ---------- Singleton + Persistencia ----------
    private static MapToggle _instance;
    void Awake()
    {
        if (_instance != null && _instance != this) { Destroy(gameObject); return; }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        var eventSystems = FindObjectsByType<UnityEngine.EventSystems.EventSystem>(FindObjectsSortMode.None);
        for (int i = 0; i < eventSystems.Length; i++)
        {
            if (i > 0) Destroy(eventSystems[i].gameObject);
        }
    }

    void OnEnable()  => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void Start()
    {
        if (mapPanel) mapPanel.SetActive(false);
        RebindPlayer(); // primera escena
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

    // ---------- Input para abrir/cerrar ----------
    void Update()
    {
        if (Keyboard.current?.cKey.wasPressedThisFrame == true) Toggle();
        if (isOpen && Keyboard.current?.escapeKey.wasPressedThisFrame == true) Close();
        if (Gamepad.current?.selectButton.wasPressedThisFrame == true) Toggle();
    }

    public void Toggle()
    {
        if (!mapPanel) return;
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
