using UnityEngine;
using System.Collections.Generic;

public class RewardManager : MonoBehaviour
{
    public List<string> rewards = new List<string>();

    public SpriteChanger lanzaUI;
    public SpriteChanger llaveUI;
    public SpriteChanger cestaUI;
    public SpriteChanger espardenyaUI;
    public SpriteChanger campanasUI;
    public SpriteChanger mielUI;
    public SpriteChanger pulseraUI;

    public static RewardManager Instance;

    void Awake()
    {
        // 🔹 Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 🔹 Inicializar referencias a la UI
        FindUI();
    }

    void FindUI()
    {
        // 🔹 Solo asignamos UI que estén vacías
        var allUI = FindObjectsOfType<SpriteChanger>(true); // true = incluye objetos inactivos
        foreach (var ui in allUI)
        {
            if (ui.name == "espardenya" && espardenyaUI == null) espardenyaUI = ui;
            else if (ui.name == "lanza" && lanzaUI == null) lanzaUI = ui;
            else if (ui.name == "cesta" && cestaUI == null) cestaUI = ui;
            else if (ui.name == "campanas" && campanasUI == null) campanasUI = ui;
            else if (ui.name == "miel" && mielUI == null) mielUI = ui;
            else if (ui.name == "llave" && llaveUI == null) llaveUI = ui;
            else if (ui.name == "pulsera" && pulseraUI == null) pulseraUI = ui;
        }
    }

    public void ReaplicarRecompensas()
    {
        foreach (string reward in rewards)
        {
            // Reaplicamos SOLO la UI, sin volver a dar recompensas
            if (reward == "Act2_Q_ESP_HasEspart")
                espardenyaUI?.SetCompletado(true);

            if (reward == "ACT2_Q_LLANCE_DONE")
                lanzaUI?.SetCompletado(true);

            if (reward == "ACT2_Q_LLANCE_GOT_KEY")
                llaveUI?.SetCompletado(true);

            if (reward == "ACT2_Q_MENJAR_GOT_MEAT")
                cestaUI?.SetCompletado(true);

            if (reward == "ACT2_Q_MENJAR_GOT_HONEY")
                mielUI?.SetCompletado(true);

            if (reward == "ACT2_Q_ESQUELLES_DONE")
                campanasUI?.SetCompletado(true);

            if (reward == "ACT2_Q_ESQUELLES_BRACELET")
                pulseraUI?.SetCompletado(true);
        }
    }

    public void ResetUI()
    {
        lanzaUI?.SetCompletado(false);
        llaveUI?.SetCompletado(false);
        cestaUI?.SetCompletado(false);
        espardenyaUI?.SetCompletado(false);
        campanasUI?.SetCompletado(false);
        mielUI?.SetCompletado(false);
        pulseraUI?.SetCompletado(false);
        rewards.Clear();
    }
    public void giveReward(string reward)
    {
        rewards.Add(reward);
        Debug.Log("Reward recibida: " + reward);

        var mc = MissionController.Instance ?? FindObjectOfType<MissionController>();

        // 🔹 Reasignar UI si alguna referencia se perdió al cambiar de escena
        FindUI();

        // -------------------- Rewards --------------------

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
            espardenyaUI?.SetCompletado(true);
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

            llaveUI?.SetCompletado(true);

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
            lanzaUI?.SetCompletado(true);

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
            mielUI?.SetCompletado(true);

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
        else if (reward == "ACT2_Q_MENJAR_GOT_MEAT")
        {
            GameManager.Change("Act2_Q_MENJAR_HasMeat");
            Debug.Log("Act2 -> Menjar: ja tinc la carn.");
            
            cestaUI?.SetCompletado(true);

            if (mc != null)
            {
                mc.SetActiveMission(
                    3,
                    "Portar la carn a Maripili",
                    "Ja tens la carn. Torna al passat i dóna-li-la a Maripili."
                );
            }
        }
        else if (reward == "ACT2_Q_ESQUELLES_START")
        {
            GameManager.Change("Act2_Q_ESQUELLES_Started");
            Debug.Log("Act2 -> Esquelles: última missió començada.");

            if (mc != null)
            {
                mc.SetActiveMission(
                    4,
                    "Buscar a Raúl a les calderetes",
                    "Maripili t'ha dit que un xic anomenat Raúl podria tindre cencerros. Ves a les calderetes a buscar-lo."
                );
            }
        }
        else if (reward == "ACT2_Q_ESQUELLES_TALKED_RAUL")
        {
            GameManager.Change("Act2_Q_ESQUELLES_TalkedToRaul");
            Debug.Log("Act2 -> Esquelles: ja he parlat amb Raúl a les calderetes.");

            if (mc != null)
            {
                mc.SetActiveMission(
                    4,
                    "Parlar amb la iaia al cementeri",
                    "Raúl et demana que averigües quin regal li agradaria a Maria Pilar. Ves al cementeri a buscar a la iaia."
                );
            }
        }
        else if (reward == "ACT2_Q_ESQUELLES_TALKED_IAIA")
        {
            GameManager.Change("Act2_Q_ESQUELLES_TalkedToIaiaPresent");
            Debug.Log("Act2 -> Esquelles: ja he parlat amb la iaia al cementeri.");

            if (mc != null)
            {
                mc.SetActiveMission(
                    4,
                    "Anar al col·legi a buscar una polsera",
                    "Al col·legi hi ha xiquets venent-ne, ves allí a parlar amb ells."
                );
            }
        }
        else if (reward == "ACT2_Q_ESQUELLES_BRACELET")
        {
            GameManager.Change("Act2_Q_ESQUELLES_HasBracelet");
            Debug.Log("Act2 -> Esquelles: ja tinc la polsera.");
            pulseraUI?.SetCompletado(true);

            if (mc != null)
            {
                mc.SetActiveMission(
                    4,
                    "Portar la polsera a Raúl",
                    "Has aconseguit una polsera al col·legi. Torna al passat i dóna-li-la a Raúl a les calderetes."
                );
            }
        }
        else if (reward == "ACT2_Q_ESQUELLES_DONE")
        {
            GameManager.Change("Act2_Q_ESQUELLES_Done");
            GameManager.Change("Act2_HasCencerros");

            campanasUI?.SetCompletado(true);

            Debug.Log("Act2 -> Esquelles completada. Joaquín ja té les esquelles.");

            if (mc != null)
            {
                mc.SetActiveMission(
                    5,
                    "Tot preparat contra l’ós",
                    "Ja tens les espardenyes, la llança, el menjar i les esquelles. Torna a parlar amb Maripili."
                );
            }
        }
        else if (reward == "ACT3_START")
        {
            if (GameManager.Check("Act3_Started"))
                return;

            GameManager.Change("Act3_Started");
            Debug.Log("Act3 -> Start (flag Act3_Started)");

            if (mc != null)
            {
                mc.SetActiveMission(
                    6,
                    "Missió activa",
                    "Segueix a Maripili fins al barranquet."
                );
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene("barranquetPasado");
        }
        else if (reward == "ACT3_PAST_BRIEFING_DONE")
        {
            if (GameManager.Check("Act3_PastBriefingDone"))
                return;

            GameManager.Change("Act3_PastBriefingDone");
            GameManager.Change("Act3_BearActive");
            Debug.Log("Act3 -> Briefing passat done (flags Act3_PastBriefingDone + Act3_BearActive)");

            if (mc != null)
            {
                mc.SetActiveMission(
                    6,
                    "Missió activa",
                    "Atrau l’ós des de la Carrasqueta fins al poble."
                );
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene("CarrasquetaPasado");
        }
        else if (reward == "ACT3_BACK_TO_PRESENT_WITH_BEAR")
        {
            if (GameManager.Check("Act3_PastCelebrationDone"))
                return;

            GameManager.Change("Act3_PastCelebrationDone");
            Debug.Log("Act3 -> Celebració passat done (flag Act3_PastCelebrationDone). Tornem al present amb l’ós actiu.");

            if (mc != null)
            {
                mc.SetActiveMission(
                    7,
                    "Missió activa",
                    "Corre al poble! Pareix que l’ós t’ha seguit."
                );
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene("Carrasqueta");
        }
        else if (reward == "ACT3_WARNED_IN_PRESENT")
        {
            if (GameManager.Check("Act3_CarrasquetaPresentWarned"))
                return;

            GameManager.Change("Act3_CarrasquetaPresentWarned");
            Debug.Log("Act3 -> Warned en present (flag Act3_CarrasquetaPresentWarned)");

            if (mc != null)
            {
                mc.SetActiveMission(
                    7,
                    "Missió activa",
                    "Ves al barranquet. La gent està reunida allí."
                );
            }
        }
        else if (reward == "ACT3_END")
        {
            if (GameManager.Check("Act3_End"))
                return;

            GameManager.Change("Act3_End");
            Debug.Log("Act3 -> End (flag Act3_End). Tornem al menú.");

            if (mc != null)
                mc.ClearMission();

            UnityEngine.SceneManagement.SceneManager.LoadScene("Fin");
        }
    }
}
