using UnityEngine;
using System;
using System.Collections.Generic;

public class MissionController : MonoBehaviour
{
    public static MissionController Instance;

    [Header("Estado de misión")]
    public bool missionStarted;
    public int missionID;

    [TextArea] public string activeMissionTitle;
    [TextArea] public string activeMissionDescription;

    [Header("Progreso narrativo")]
    public List<string> progressFlags;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (progressFlags == null)
            progressFlags = new List<string>();
    }

    public bool isInMission()
    {
        return missionStarted;
    }

    public int mission_ID()
    {
        return missionID;
    }

    // 🔹 NUEVO: función para activar / actualizar misión
    public void SetActiveMission(int id, string title, string description)
    {
        missionID = id;
        missionStarted = true;
        activeMissionTitle = title;
        activeMissionDescription = description;
    }

    // 🔹 Opcional: limpiar misión
    public void ClearMission()
    {
        missionStarted = false;
        missionID = 0;
        activeMissionTitle = "";
        activeMissionDescription = "";
    }
}
