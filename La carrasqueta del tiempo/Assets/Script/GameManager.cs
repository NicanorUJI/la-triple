using UnityEngine;
using System.Linq;

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
        }
    }

    public static bool Check(string condicion)
    {
        /*Mirar si el primer carácter es !(significa NO ha hecho tal cosa)
        Si la condición pone "="-- > Split
        Mirar si el numero al que tiene que ser igual se corresponde con el guardado
        Devolver si al final se cumple o no*/

        missionController = FindObjectOfType<MissionController>();

        if (missionController.progressFlags.Contains(condicion))
            { return true; }

        return false;

    }

    public static void Change(string condicion)
    {
        missionController = FindObjectOfType<MissionController>();
        if (!missionController.progressFlags.Contains(condicion))
        { 
            missionController.progressFlags.Add(condicion); 
        }

    }
}
