using UnityEngine;
using Domino.Game;
using Domino.Core;
using Domino.AI;
using System.Collections;

public class DominoGameController : MonoBehaviour
{
    public DominoUI ui; // arrastra el Canvas

    TurnManager tm;

    void Start()
    {
        tm = new TurnManager();
        ui.turn = tm;
        ui.HookEvents();

        tm.OnLog += Debug.Log;
        tm.OnTurnChanged += OnTurnChanged;
        tm.OnMatchEnded += (_, _) => StopAllCoroutines();

        tm.StartMatch(playerCount: 3);
        ui.InitAndDraw();
    }

    void OnTurnChanged(int playerIndex)
    {
        // Si no es el jugador humano (P0), ejecuta IA automáticamente
        if (playerIndex != 0)
            StartCoroutine(NpcTurnRoutine(playerIndex));
    }

    IEnumerator NpcTurnRoutine(int npcIndex)
    {
        yield return new WaitForSeconds(0.6f);

        var npc = tm.Players[npcIndex];
        var move = Domino.AI.NpcAgent.ChooseMove(tm.Board, npc.Hand);

        if (move != null)
        {
            var (tile, toLeft) = move.Value;
            if (toLeft) tm.TryPlayLeft(npcIndex, tile);
            else tm.TryPlayRight(npcIndex, tile);
            yield break;
        }

        // No tiene jugadas → intentar robar; si tras robar tiene jugada, jugar
        int beforeCount = npc.Hand.Count;
        tm.DrawOrPass(npcIndex); // esto puede avanzar turno. Si sigue siendo su turno y ahora puede, jugar.

        // Si después de DrawOrPass sigue siendo su turno (porque ahora tiene jugada),
        // lo intentamos de nuevo una vez.
        if (tm.State == MatchState.Playing && tm.CurrentPlayerIndex == npcIndex && npc.Hand.Count > beforeCount)
        {
            yield return new WaitForSeconds(0.2f);
            move = Domino.AI.NpcAgent.ChooseMove(tm.Board, npc.Hand);
            if (move != null)
            {
                var (tile2, toLeft2) = move.Value;
                if (toLeft2) tm.TryPlayLeft(npcIndex, tile2);
                else tm.TryPlayRight(npcIndex, tile2);
            }
        }
    }
}
