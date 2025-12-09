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
            // Evitar que se dispare dos veces
            if (GameManager.Check("Act1_FirstTravelPast"))
                return;

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

        // aquí después añadiremos otros rewards
    }
}
