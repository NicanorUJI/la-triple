using UnityEngine;
using System;
using System.Collections.Generic;

public class MissionController : MonoBehaviour
{
    public bool missionStarted; //true: hay alguna mision activa; false: pues no
    public int missionID; //cada uno de los objetos tiene una mision ID. o algo. no se si esto se va a usar o que

    public List<string> progressFlags;


    public bool isInMission()
    {
        return missionStarted;
    }

    public int mission_ID()
    {
        return missionID;
    }
}
