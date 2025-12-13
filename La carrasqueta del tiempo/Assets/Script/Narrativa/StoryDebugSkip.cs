using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryDebugSkip : MonoBehaviour
{
    [Header("Activar/desactivar cheats")]
    public bool enableDebugSkips = true;

    [Header("Nombre de escenas (rellena en el inspector)")]
    public string scenePastTown;      // p.ej. "Poble_Passat"
    public string sceneCementeri;     // p.ej. "Cementeri_Present"
    public string sceneCole;          // p.ej. "Cole_Present"

    private void Update()
    {
        if (!enableDebugSkips) return;

        // F1 = marcar misiones como completadas y dejarte en el poble passat con Esquelles empezada
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SkipToEsquellesIntro();
        }

        // F2 = ir directo a cementeri con Esquelles en progreso
        if (Input.GetKeyDown(KeyCode.F2))
        {
            SkipToCementeri();
        }

        // F3 = ir directo al cole con la parte de la iaia ya hecha
        if (Input.GetKeyDown(KeyCode.F3))
        {
            SkipToCole();
        }
    }

    private void MarkAct2QuestsDone()
    {
        // Misiones Acto 2 ya terminadas
        GameManager.Change("Act2_Q_ESP_Done");
        GameManager.Change("Act2_Q_LLANCE_Done");
        GameManager.Change("Act2_Q_MENJAR_Done");

        // Opcional: marcar también los objetos como obtenidos, por si los usas en checks
        GameManager.Change("Act2_Q_ESP_HasEspart");
        GameManager.Change("Act2_Q_LLANCE_HasLance");
        GameManager.Change("Act2_Q_MENJAR_HasMeat");
        GameManager.Change("Act2_Q_MENJAR_HasHoney");
    }

    private void SkipToEsquellesIntro()
    {
        MarkAct2QuestsDone();

        // Marcar que Esquelles ya está empezada (como si hubieras hecho el diálogo con Maripili)
        GameManager.Change("Act2_Q_ESQUELLES_Started");

        // Opcional: actualizar panel de misión
        var mc = MissionController.Instance ?? FindObjectOfType<MissionController>();
        if (mc != null)
        {
            mc.SetActiveMission(
                4,
                "Buscar a Raúl a les calderetes",
                "Has completat les tres missions. Maripili t'ha dit que busques a Raúl a les calderetes per aconseguir les esquelles."
            );
        }

        if (!string.IsNullOrEmpty(scenePastTown))
        {
            SceneManager.LoadScene(scenePastTown);
        }

        Debug.Log("[DEBUG] SkipToEsquellesIntro ejecutado.");
    }

    private void SkipToCementeri()
    {
        // Suponemos que ya hablaste con Raúl al riu
        MarkAct2QuestsDone();
        GameManager.Change("Act2_Q_ESQUELLES_Started");
        GameManager.Change("Act2_Q_ESQUELLES_TalkedToRaul");

        var mc = MissionController.Instance ?? FindObjectOfType<MissionController>();
        if (mc != null)
        {
            mc.SetActiveMission(
                4,
                "Parlar amb la iaia al cementeri",
                "Raúl t'ha demanat que averigües quin regal li agradaria a Maria Pilar. Ves al cementeri a parlar amb la teua iaia."
            );
        }

        if (!string.IsNullOrEmpty(sceneCementeri))
        {
            SceneManager.LoadScene(sceneCementeri);
        }

        Debug.Log("[DEBUG] SkipToCementeri executat.");
    }

    private void SkipToCole()
    {
        // Suponemos que ja has parlat amb la iaia al cementeri
        MarkAct2QuestsDone();
        GameManager.Change("Act2_Q_ESQUELLES_Started");
        GameManager.Change("Act2_Q_ESQUELLES_TalkedToRaul");
        GameManager.Change("Act2_Q_ESQUELLES_TalkedToIaiaPresent");

        var mc = MissionController.Instance ?? FindObjectOfType<MissionController>();
        if (mc != null)
        {
            mc.SetActiveMission(
                4,
                "Anar al col·legi a buscar la polsera",
                "La iaia t'ha contat com es va enamorar del iaio i t'ha parlat de les polseres del col·legi."
            );
        }

        if (!string.IsNullOrEmpty(sceneCole))
        {
            SceneManager.LoadScene(sceneCole);
        }

        Debug.Log("[DEBUG] SkipToCole executat.");
    }
}
