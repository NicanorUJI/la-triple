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
        else if (reward == "ACT2_Q_LLANCE_START")
        {
            GameManager.Change("Act2_Q_LLANCE_Started");

            if (MissionController.Instance != null)
            {
                MissionController.Instance.SetActiveMission(
                    2,
                    "Conseguir la llança",
                    "Ves a l’ermita de Santa Bàrbara (la de baix) i mira si pots aconseguir la llança."
                );
            }
        }
        else if (reward == "ACT2_Q_LLANCE_WON_SOLTERONA")
        {
            GameManager.Change("Act2_Q_LLANCE_WonSolterona");
            Debug.Log("Act2 -> 'solterona' guanyada (sense minijoc).");
        }
        else if (reward == "ACT2_Q_LLANCE_GOT_CLUE")
        {
            GameManager.Change("Act2_Q_LLANCE_HasClue");

            if (MissionController.Instance != null)
            {
                MissionController.Instance.SetActiveMission(
                    2,
                    "Buscar la clau de l’ermita",
                    "L’alcalde t’ha dit que la clau està amagada en una pedra al costat de la creu de l’ermita."
                );
            }
        }
        else if (reward == "ACT2_Q_LLANCE_GOT_KEY")
        {
            GameManager.Change("Act2_Q_LLANCE_HasKey");
            Debug.Log("Act2 -> Llança: ja tinc la clau de l'ermita.");

            if (mc != null)
            {
                mc.SetActiveMission(
                    2,
                    "Obrir l’ermita",
                    "Ja tens la clau. Torna a l’ermita de Santa Bàrbara i intenta obrir la porta."
                );
            }
        }
        else if (reward == "ACT2_Q_LLANCE_DONE")
        {
            GameManager.Change("Act2_Q_LLANCE_Done");

            if (MissionController.Instance != null)
            {
                MissionController.Instance.ClearMission();
            }
        }
        else if (reward == "ACT2_Q_MENJAR_START")
        {
            GameManager.Change("Act2_Q_MENJAR_Started");
            Debug.Log("Act2 -> Quest MENJAR STARTED");

            if (mc != null)
            {
                mc.SetActiveMission(
                    3,
                    "Buscar menjar",
                    "Maripili creu que podem calmar l'ós donant-li de menjar. Pregunta al bar si pots aconseguir alguna cosa per a ell."
                );
            }
        }
        else if (reward == "ACT2_Q_MENJAR_DONE")
        {
            GameManager.Change("Act2_Q_MENJAR_Done");
            Debug.Log("Act2 -> Quest MENJAR COMPLETED");

            if (mc != null)
            {
                mc.ClearMission();
            }
        }
        else if (reward == "ACT2_Q_MENJAR_GOT_HONEY")
        {
            GameManager.Change("Act2_Q_MENJAR_HasHoney");
            Debug.Log("Act2 -> Menjar: ja tinc el pot de mel.");

            if (mc != null)
            {
                mc.SetActiveMission(
                    3,
                    "Portar la mel a Maripili",
                    "Has guanyat el pot de mel al dominó. Torna al passat i dóna-li'l a Maripili."
                );
            }
        }

        else if (reward == "ACT2_Q_MENJAR_NEEDS_MEAT")
        {
            GameManager.Change("Act2_Q_MENJAR_NeedsMeat");
            Debug.Log("Act2 -> MENJAR: ara cal aconseguir carn.");

            if (mc != null)
            {
                mc.SetActiveMission(
                    3,
                    "Buscar carn per a l’ós",
                    "Maripili diu que la mel és poca. Ves cap a la plaça i busca la carnisseria."
                );
            }
        }
        else if (reward == "ACT2_Q_MENJAR_START_MORRA")
        {
            GameManager.Change("Act2_Q_MENJAR_MorraStarted");
            Debug.Log("Act2 -> Menjar: iniciada fase de la carn (Morra).");

            if (mc != null)
            {
                mc.SetActiveMission(
                    3,
                    "Aconseguir carn per a l’ós",
                    "Maripili diu que la mel és poca. Ves a la plaça i busca la carnisseria o algun lloc on aconseguir carn."
                );
            }
        }
    }
}
