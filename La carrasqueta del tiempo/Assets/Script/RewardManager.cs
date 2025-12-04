using UnityEngine;
using System;
using System.Collections.Generic;

public class RewardManager : MonoBehaviour
{
    public List<string> rewards = new List<string>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void giveReward(string reward)
    {
        rewards.Add(reward);
    }
}
