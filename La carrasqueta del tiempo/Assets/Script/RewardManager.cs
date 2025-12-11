using UnityEngine;
using System.Collections.Generic;

public class RewardManager : MonoBehaviour
{
    public List<string> rewards = new List<string>();

    public void giveReward(string reward)
    {
        rewards.Add(reward);
        Debug.Log("Reward recibida: " + reward);

        var mc = MissionController.Instance ?? FindObjectOfType<MissionController>();

        if (reward == "ACT1_INTRO_END")
        {
            GameManager.Change("Act1_IntroDone");
            Debug.Log("Intro Acto 1 completada -> flag Act1_IntroDone");

            // 🔹 Guardamos la misión en MissionController
            if (mc != null)
            {
                mc.SetActiveMission(
                    1,
                    "Missió activa",
                    "Ves a la Carrasqueta i busca la cistella per a la iaia."
                );
            }
        }
        else if (reward == "ACT1_REACHED_CARRASQUETA")
        {
            GameManager.Change("Act1_ReachedCarrasqueta");
            Debug.Log("Has arribat a la Carrasqueta -> flag Act1_ReachedCarrasqueta");

            if (mc != null)
            {
                mc.SetActiveMission(
                    1,
                    "Missió activa",
                    "Busca la cistella que s'ha deixat la iaia a la Carrasqueta."
                );
            }
        }
        else if (reward == "ACT1_FOUND_BASKET")
        {
            GameManager.Change("Act1_FoundBasket");
            Debug.Log("Cistella trobada -> flag Act1_FoundBasket");

            if (mc != null)
            {
                mc.SetActiveMission(
                    2,
                    "Missió activa",
                    "Abans de tornar al poble, mira què és això que brilla en la carrasqueta."
                );
            }

            // ocultar sprite
            var basket = GameObject.FindWithTag("Cesta");
            if (basket != null)
            {
                var sprites = basket.GetComponentsInChildren<SpriteRenderer>();
                foreach (var sr in sprites)
                    sr.enabled = false;
            }
        }
        else if (reward == "ACT1_FIRST_TRAVEL_PAST")
        {
            // Solo la primera vez marcamos el flag y la misión
            if (!GameManager.Check("Act1_FirstTravelPast"))
            {
                GameManager.Change("Act1_FirstTravelPast");
                Debug.Log("Primer viatge al passat -> flag Act1_FirstTravelPast");

                if (mc != null)
                {
                    mc.SetActiveMission(
                        4,
                        "Missió activa",
                        "Explora el poble en el passat i parla amb la gent."
                    );
                }
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene("CarrasquetaPasado");
        }
        else if (reward == "ACT2_MARIPILI_INTRO_END")
        {
            if (GameManager.Check("Act2_MaripiliIntroDone"))
                return;

            GameManager.Change("Act2_MaripiliIntroDone");
            Debug.Log("Intro Maripili passat -> flag Act2_MaripiliIntroDone");

            if (mc != null)
            {
                mc.SetActiveMission(
                    5,
                    "Missió activa",
                    "Ajuda a Maripili a preparar-se per a espantar l'ós."
                );
            }
        }
        else if (reward == "ACT2_Q_LLANCE_START")
        {
            if (!GameManager.Check("Act2_Quest_Llanca_Started"))
                GameManager.Change("Act2_Quest_Llanca_Started");

            Debug.Log("Act2 -> Quest Llança STARTED");

            if (mc != null)
            {
                mc.SetActiveMission(
                    7,
                    "Missió activa",
                    "Busca la manera d’aconseguir la clau de l’ermita per a agarrar la llança."
                );
            }
        }
        else if (reward == "ACT2_Q_MENJAR_START")
        {
            if (!GameManager.Check("Act2_Quest_Menjar_Started"))
                GameManager.Change("Act2_Quest_Menjar_Started");

            Debug.Log("Act2 -> Quest Menjar STARTED");

            if (mc != null)
            {
                mc.SetActiveMission(
                    8,
                    "Missió activa",
                    "Parla amb la gent del poble per aconseguir menjar per a l’ós."
                );
            }
        }
        else if (reward == "ACT2_Q_ESP_START")
        {
            GameManager.Change("Act2_Q_ESP_Started");
            if (mc != null)
                mc.SetActiveMission(6, "Missió activa",
                    "Ves a la tenda d'espardenyes i ajuda al sabater.");
        }
        else if (reward == "ACT2_Q_ESP_GOT_MATERIAL")
        {
            GameManager.Change("Act2_Q_ESP_HasEspart");
            if (mc != null)
                mc.SetActiveMission(6, "Missió activa",
                    "Torna amb l'espart a parlar amb Maripili.");
        }
        else if (reward == "ACT2_Q_ESP_DONE")
        {
            GameManager.Change("Act2_Q_ESP_Done");
            Debug.Log("Act2 -> Quest Espardenyes COMPLETED");

            mc?.SetActiveMission(5, "Missió activa",
                    "Parla amb Maripili i tria una altra cosa per a preparar contra l'ós.");
        }

        // aquí después añadiremos otros rewards
    }
}
