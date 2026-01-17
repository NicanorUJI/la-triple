using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public float playerX;
    public float playerY;
    public float playerZ;
    public string sceneName;

    // 🔹 Rewards / flags
    public List<string> rewards;

    // Audio
    public float volumenAmbiente;
    public float volumenEfectos;
}
