using UnityEngine;

public class PresentHermitaDoor : MonoBehaviour
{
    [Header("Flags de historia")]
    public string keyFlag = "Act2_Q_LLANCE_HasKey";
    public string doorOpenFlag = "SB_Present_DoorOpen";

    [Header("Referencias visuales")]
    public GameObject closedVisual;    // Lo puedes dejar en null si la puerta cerrada está pintada en el fondo
    public GameObject openVisual;      // Aquí tu sprite de PuertaAbierta

    [Header("Colisión y TP")]
    public Collider2D closedCollider;  // Collider que bloquea el paso (puede ser el TilemapCollider de la pared)
    public GameObject teleportTrigger;

    private bool playerInside = false;
    private bool isOpen = false;

    private void Start()
    {
        isOpen = GameManager.Check(doorOpenFlag);
        Debug.Log($"[Door] Start. isOpen={isOpen}, doorOpenFlag={doorOpenFlag}");

        ApplyState();
    }

    private void ApplyState()
    {
        if (closedVisual != null)
            closedVisual.SetActive(!isOpen);

        if (openVisual != null)
            openVisual.SetActive(isOpen);

        if (closedCollider != null)
            closedCollider.enabled = !isOpen;

        if (teleportTrigger != null)
            teleportTrigger.SetActive(isOpen);

        Debug.Log($"[Door] ApplyState -> isOpen={isOpen}, openVisual={(openVisual ? openVisual.activeSelf : (bool?)null)}, tp={(teleportTrigger ? teleportTrigger.activeSelf : (bool?)null)}");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            Debug.Log("[Door] Player entered trigger.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            Debug.Log("[Door] Player left trigger.");
        }
    }

    private void Update()
    {
        if (!playerInside)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("[Door] E pressed near door.");

            if (isOpen)
            {
                Debug.Log("[Door] Already open. Nothing else to do.");
                return;
            }

            bool hasKey = GameManager.Check(keyFlag);
            Debug.Log($"[Door] hasKey={hasKey}, keyFlag={keyFlag}");

            if (!hasKey)
            {
                Debug.Log("[Door] La porta està tancada, encara no tinc la clau.");
                return;
            }

            // Abrir puerta
            isOpen = true;
            GameManager.Change(doorOpenFlag);
            Debug.Log("[Door] Opening door, setting flag " + doorOpenFlag);

            ApplyState();
        }
    }
}
