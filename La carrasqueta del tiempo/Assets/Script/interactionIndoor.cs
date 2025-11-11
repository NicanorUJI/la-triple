using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class InteractionIndoor : MonoBehaviour
{
    private IInteractable interactableInRange = null;
    public GameObject interactionIcon;
    private bool llave1 = false;
    private bool llave2 = false;

    [Header("Prefabs de puertas abiertas")]
    public GameObject puerta1AbiertaPrefab; // Prefab de la puerta abierta
    public Vector3 puerta1AbiertaPos; // Posición específica donde aparecerá

    public GameObject puerta2AbiertaPrefab; // Opcional para puerta2
    public Vector3 puerta2AbiertaPos; // Posición específica para puerta2

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
            // --- Puerta 2 ---
            else if (objeto.CompareTag("Puerta2"))
            {
                if (llave2)
                {
                    Debug.Log("Tienes la llave 2 — cruzando la puerta.");
                    SceneManager.LoadScene("Plaza"); // Cambia por tu escena
                }
                else
                {
                    Debug.Log("No puedes abrir la puerta 2 sin la llave 2.");
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

            // Instanciar la puerta abierta en posición específica
            if (puerta1AbiertaPrefab != null)
            {
                Instantiate(puerta1AbiertaPrefab, puerta1AbiertaPos, Quaternion.identity);
            }

            return;
        }

        if (collision.CompareTag("Llave2"))
        {
            Debug.Log("Has recogido la llave 2");
            llave2 = true;
            Destroy(collision.gameObject);
            interactionIcon.SetActive(false);

            // Instanciar la puerta abierta en posición específica (opcional)
            if (puerta2AbiertaPrefab != null)
            {
                Instantiate(puerta2AbiertaPrefab, puerta2AbiertaPos, Quaternion.identity);
            }

            return;
        }

        // --- Detectar puertas ---
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

        if (collision.CompareTag("Puerta2"))
        {
            if (!llave2 && collision.TryGetComponent(out IInteractable interactable2) && interactable2.CanInteract())
            {
                interactableInRange = interactable2;
                interactionIcon.SetActive(true);
            }
            else if (llave2)
            {
                interactableInRange = collision.GetComponent<IInteractable>();
                interactionIcon.SetActive(false);
            }
            return;
        }

        // --- Otros objetos interactuables ---
        if (collision.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
        {
            interactableInRange = interactable;
            interactionIcon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Puerta1") || collision.CompareTag("Puerta2") || collision.GetComponent<IInteractable>() != null)
        {
            interactableInRange = null;
            interactionIcon.SetActive(false);
        }
    }
}
