using System;
using System.Collections.Generic;
using UnityEngine;
using Domino.Core;

namespace Domino.Game
{
    public enum MatchState { None, Dealing, Playing, Ended }

    public class TurnManager
    {
        public MatchState State { get; private set; } = MatchState.None;

        public readonly List<PlayerState> Players = new List<PlayerState>();
        public readonly BoardState Board = new BoardState();
        public DominoDeck Deck { get; private set; }

        public int CurrentPlayerIndex { get; private set; }
        public int PlayerCount => Players.Count;

        // Eventos para UI/Controller
        public event Action<int> OnTurnChanged;
        public event Action<BoardState> OnBoardUpdated;
        public event Action<string> OnLog;
        public event Action<int, string> OnMatchEnded; // (winnerIndex o -1 si empate, reason)

        int _consecutivePasses = 0;
        int _moveCounter = 0;

        public void StartMatch(int playerCount = 3, int seed = 0)
        {
            _consecutivePasses = 0;
            _moveCounter = 0;

            if (playerCount < 2) playerCount = 2;

            State = MatchState.Dealing;
            Players.Clear();
            for (int i = 0; i < playerCount; i++) Players.Add(new PlayerState());

            Deck = new DominoDeck(seed);
            Board.Clear();

            // Reparto: 7 fichas cada uno
            for (int r = 0; r < 7; r++)
                for (int p = 0; p < playerCount; p++)
                    if (Deck.TryDraw(out var t)) Players[p].Add(t);

            // Elegir salida: doble más alto; si nadie tiene, aleatorio
            if (!TryFindStartingPlayerWithHighestDouble(out int starter, out DominoTile startTile))
            {
                starter = UnityEngine.Random.Range(0, playerCount);
                // Si sale sin doble, coloca la primera ficha de su mano
                startTile = Players[starter].Hand[0];
            }

            // Colocar la salida y quitar de la mano
            Board.PlaceFirst(startTile);
            Players[starter].Remove(startTile);
            OnLog?.Invoke($"Sale P{starter} con {startTile}");
            OnBoardUpdated?.Invoke(Board);

            // Empieza el siguiente jugador en sentido horario
            CurrentPlayerIndex = NextIndex(starter);
            State = MatchState.Playing;
            OnTurnChanged?.Invoke(CurrentPlayerIndex);
        }

        // Busca el jugador con el doble más alto (6..0)
        bool TryFindStartingPlayerWithHighestDouble(out int idx, out DominoTile tile)
        {
            for (int pip = 6; pip >= 0; pip--)
            {
                var d = new DominoTile(pip, pip);
                for (int p = 0; p < Players.Count; p++)
                {
                    if (Players[p].HasTile(d.A, d.B))
                    {
                        idx = p; tile = d; return true;
                    }
                }
            }
            idx = -1; tile = default;
            return false;
        }

        public int NextIndex(int i) => (i + 1) % Players.Count;

        public void TryPlayLeft(int player, DominoTile tile)
        {
            if (State != MatchState.Playing || player != CurrentPlayerIndex) return;
            if (!Board.CanPlaceLeft(tile)) { OnLog?.Invoke("Movimiento inválido (izq)."); return; }

            Board.PlaceLeft(tile);
            Players[player].Remove(tile);
            OnBoardUpdated?.Invoke(Board);

            _moveCounter++;
            _consecutivePasses = 0;

            CheckEndOrAdvance();
        }

        public void TryPlayRight(int player, DominoTile tile)
        {
            if (State != MatchState.Playing || player != CurrentPlayerIndex) return;
            if (!Board.CanPlaceRight(tile)) { OnLog?.Invoke("Movimiento inválido (der)."); return; }

            Board.PlaceRight(tile);
            Players[player].Remove(tile);
            OnBoardUpdated?.Invoke(Board);

            _moveCounter++;
            _consecutivePasses = 0;

            CheckEndOrAdvance();
        }

        public void DrawOrPass(int player)
        {
            if (State != MatchState.Playing || player != CurrentPlayerIndex) return;

            // Si puede jugar, no debería venir aquí, pero por seguridad:
            if (PlayerHasAnyMove(player))
            {
                OnLog?.Invoke($"P{player} tiene jugadas; usa TryPlay en vez de DrawOrPass.");
                return;
            }

            // Roba hasta que pueda jugar o hasta que no queden fichas
            bool drew = false;
            if (Deck.TryDraw(out var t))
            {
                Players[player].Add(t);
                OnLog?.Invoke($"P{player} roba {t}");
            }
            else
            {
                OnLog?.Invoke($"P{player} no puede robar (mazo vacío)");
            }

            // Luego de robar, si todavía no puede jugar → pasa
            if (!PlayerHasAnyMove(player))
            {
                _consecutivePasses++;
                OnLog?.Invoke($"P{player} pasa");
                AdvanceTurnAndCheckBlock();
            }
            else
            {
                // Tiene una jugada ahora, pero termina su turno igual
                _consecutivePasses = 0;
                AdvanceTurn();
            }

            if (!PlayerHasAnyMove(player))
            {
                // Pase (no hay jugada ni tras robar; o no había fichas para robar)
                _consecutivePasses++;
                OnLog?.Invoke($"P{player} pasa");
                AdvanceTurnAndCheckBlock();
            }
            else
            {
                // Tiene jugada luego de robar: por simplicidad, termina su fase y toca a UI/IA decidir
                _consecutivePasses = 0;
                AdvanceTurn(); // si querés forzar que juegue inmediatamente, podemos hacerlo luego
            }
        }

        void AdvanceTurnAndCheckBlock()
        {
            // ¿bloqueo? (todos pasaron consecutivamente y no hay fichas en mazo)
            if (_consecutivePasses >= Players.Count && Deck.Count == 0)
            {
                EndByBlockAndPoints();
                return;
            }
            AdvanceTurn();
        }

        void CheckEndOrAdvance()
        {
            if (Players[CurrentPlayerIndex].IsEmpty)
            {
                State = MatchState.Ended;
                OnMatchEnded?.Invoke(CurrentPlayerIndex, "Jugador sin fichas (victoria)");
                return;
            }

            // Watchdog opcional por seguridad (evita loops infinitos por bugs)
            if (_moveCounter > 500)
            {
                EndByBlockAndPoints();
                return;
            }

            AdvanceTurn();
        }

        void AdvanceTurn()
        {
            CurrentPlayerIndex = NextIndex(CurrentPlayerIndex);
            OnTurnChanged?.Invoke(CurrentPlayerIndex);
        }

        bool PlayerHasAnyMove(int playerIndex)
        {
            var hand = Players[playerIndex].Hand;
            var moves = RulesEngine.GetValidMoves(hand, Board);
            return moves.Count > 0;
        }

        void EndByBlockAndPoints()
        {
            // Gana quien tenga menor suma de pips
            int bestIdx = -1;
            int bestPips = int.MaxValue;
            bool tie = false;

            for (int i = 0; i < Players.Count; i++)
            {
                int ps = Players[i].PipSum;
                if (ps < bestPips)
                {
                    bestPips = ps;
                    bestIdx = i;
                    tie = false;
                }
                else if (ps == bestPips)
                {
                    tie = true;
                }
            }

            State = MatchState.Ended;
            if (tie) OnMatchEnded?.Invoke(-1, "Bloqueo: empate por puntos");
            else     OnMatchEnded?.Invoke(bestIdx, $"Bloqueo: gana P{bestIdx} por menor suma ({bestPips})");
        }
    }
}
