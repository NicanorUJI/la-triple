using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public string lastExitName;

    private static MissionController missionController;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        EnsureMissionController();
    }

    // 🔹 Localiza el MissionController y asegura la lista de flags
    private static void EnsureMissionController()
    {
        if (missionController == null)
        {
            missionController = FindObjectOfType<MissionController>();
            if (missionController == null)
            {
                Debug.LogWarning("GameManager: no se encontró MissionController en la escena.");
                return;
            }
        }

        if (missionController.progressFlags == null)
        {
            missionController.progressFlags = new List<string>();
        }
    }

    public static bool Check(string condicion)
    {
        EnsureMissionController();
        if (missionController == null)
            return false;

        if (missionController.progressFlags == null)
            return false;

        return missionController.progressFlags.Contains(condicion);
    }

    public static void Change(string condicion)
    {
        EnsureMissionController();
        if (missionController == null)
            return;

        if (missionController.progressFlags == null)
            missionController.progressFlags = new List<string>();

        if (!missionController.progressFlags.Contains(condicion))
        {
            missionController.progressFlags.Add(condicion);
            Debug.Log("GameManager.Change -> añadido flag: " + condicion);
        }
    }
}
