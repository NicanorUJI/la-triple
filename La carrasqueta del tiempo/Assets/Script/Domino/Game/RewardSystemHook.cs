using System;
using UnityEngine;

namespace Domino.Game
{
    public static class RewardSystemHook
    {
        // Evento para que el juego principal lo capture
        public static Action<string> OnRewardGranted;
        public static Action OnExitMinigame;

        public static void Grant(string itemName)
        {
            Debug.Log($"[Recompensa] Otorgado: {itemName}");
            OnRewardGranted?.Invoke(itemName);
        }
    }
}
