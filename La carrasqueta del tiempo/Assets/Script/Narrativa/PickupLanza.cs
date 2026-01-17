using UnityEngine;
using UnityEngine.SceneManagement;

public class PickupLanza : MonoBehaviour
{
    [Header("Historia")]
    public string lanceFlag = "Act2_Q_LLANCE_HasLance";

    [Tooltip("Escena a la que tornar després d'agafar la llança. Deixa-ho buit si no vols canviar d'escena.")]
    public string returnScene;

    private bool playerInside = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }

    private void Update()
    {
        if (!playerInside)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 1) Marcar flag de historia
            GameManager.Change(lanceFlag);
            Debug.Log("[Llança] Flag marcat: " + lanceFlag);

            // 2) Actualizar la misión en el panel
            var mc = MissionController.Instance ?? FindObjectOfType<MissionController>();
            if (mc != null)
            {
                mc.SetActiveMission(
                    2,                       // mismo ID que uses para la quest de la llança
                    "Missió activa",
                    "Ja tens la llança. Torna al passat i porta-la a Maripili."
                );
            }

            // 3) Quitar la lanza de la escena
            gameObject.SetActive(false);

            // 4) Opcional: cambiar de escena después de recogerla
            if (!string.IsNullOrEmpty(returnScene))
            {
                SceneManager.LoadScene(returnScene);
            }
        }
    }
}
