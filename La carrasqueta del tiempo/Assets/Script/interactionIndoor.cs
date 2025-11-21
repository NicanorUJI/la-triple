using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InteractionIndoor : MonoBehaviour
{
    private IInteractable interactableInRange = null;
    public GameObject interactionIcon;
    private bool llave1 = false;
    public GameObject PuertaAbierta;
    private Vector3 puertaPos = new(-0.23f, -0.14f, -3.94f);

    void Start()
    {
        interactionIcon.SetActive(false);
    }

    void Update()
    {
        // Presionar E para interactuar
        if (Keyboard.current?.eKey.wasPressedThisFrame == true && interactableInRange != null)
        {
            GameObject objeto = ((MonoBehaviour)interactableInRange).gameObject;

            // --- Puerta 1 ---
            if (objeto.CompareTag("Puerta1"))
            {
                if (llave1)
                {
                    Debug.Log("Tienes la llave 1 — cruzando la puerta.");
                    SceneManager.LoadScene("Plaza"); // Cambia por tu escena
                }
                else
                {
                    Debug.Log("No puedes abrir la puerta 1 sin la llave 1.");
                }
            }
            // --- Otros objetos interactuables ---
            else
            {
                interactableInRange.Interact();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // --- Recoger llaves ---
        if (collision.CompareTag("Llave1"))
        {
            Debug.Log("Has recogido la llave 1");
            llave1 = true;
            Destroy(collision.gameObject);
            interactionIcon.SetActive(false);

            // 👉 Buscar el objeto vacío "Puerta1" y eliminarlo
            GameObject puertaTrigger = GameObject.FindGameObjectWithTag("Puerta1");
            if (puertaTrigger != null)
                Destroy(puertaTrigger);

            // 👉 Instanciar la puerta abierta
            if (PuertaAbierta != null)
            {
                Instantiate(PuertaAbierta, puertaPos, Quaternion.identity);
            }

            return;
        }


        // --- Detectar puerta cerrada ---
        if (collision.CompareTag("Puerta1"))
        {
            if (!llave1 && collision.TryGetComponent(out IInteractable interactable1) && interactable1.CanInteract())
            {
                interactableInRange = interactable1;
                interactionIcon.SetActive(true);
            }
            else if (llave1)
            {
                interactableInRange = collision.GetComponent<IInteractable>();
                interactionIcon.SetActive(false);
            }
            return;
        }

        // --- Cualquier otro objeto interactuable ---
        if (collision.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
        {
            interactableInRange = interactable;
            interactionIcon.SetActive(true);
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Puerta1") || collision.GetComponent<IInteractable>() != null)
        {
            interactableInRange = null;
            interactionIcon.SetActive(false);
        }
    }
}
