using UnityEngine;
using UnityEngine.UI;
using Domino.Core;
using Domino.Game;
using Domino.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;

public class DominoUI : MonoBehaviour
{
    [Header("Refs")]
    public TurnManager turn;
    public RectTransform panelMesa;
    public Transform panelMano;
    public Button btnRobar;
    public Button btnPasar;
    public TMP_Text txtTurno;
    public TMP_Text txtMensaje;
    public Canvas mainCanvas;

    [Header("Fin de partida")]
    public GameObject panelFinPartida;
    public TMP_Text txtResultado;
    public TMP_Text txtRecompensa;
    public Button btnReintentar;
    public Button btnSalir;

    [Header("Prefabs")]
    public DominoTileView tileViewPrefab;

    // cache de views para limpiar rápido
    readonly List<DominoTileView> _manoViews = new();
    readonly List<DominoTileView> _mesaViews = new();

    int PlayerIndex => 0; // jugador humano = P0

    [Header("Scroll Mesa")]
    public UnityEngine.UI.ScrollRect scrollMesa;
    public RectTransform contenidoMesa;

    void Awake()
    {
        btnRobar.onClick.AddListener(() => turn.DrawOrPass(PlayerIndex));
        btnPasar.onClick.AddListener(() => turn.DrawOrPass(PlayerIndex));
    }

    public void HookEvents()
    {
        turn.OnBoardUpdated += _ => RedrawBoard();
        turn.OnTurnChanged += OnTurnChanged;
        turn.OnMatchEnded += (w, reason) =>
        {
            ShowEndPanel(w, reason);
        };
    }

    public void InitAndDraw()
    {
        RedrawBoard();
        RedrawHand(); // mano inicial
        OnTurnChanged(turn.CurrentPlayerIndex);
    }

    void OnTurnChanged(int p)
    {
        string jugadorActual = "";
        if (p == 0) jugadorActual = "Joaquin";
        else if (p == 1) jugadorActual = "Encarna";
        else if (p == 2) jugadorActual = "Josefina";
        else jugadorActual = "M. Amparo";

        txtTurno.text = $"És el torn de: {jugadorActual} !";
        txtMensaje.text = "";
        RedrawHand(); // re-evalúa interactuabilidad de fichas
    }

    void RedrawBoard()
    {
        foreach (var v in _mesaViews) Destroy(v.gameObject);
        _mesaViews.Clear();

        foreach (var t in turn.Board.Chain)
        {
            var v = Instantiate(tileViewPrefab, contenidoMesa);
            v.Setup(t, mainCanvas, false);
            _mesaViews.Add(v);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(contenidoMesa);
        if (scrollMesa) scrollMesa.normalizedPosition = new Vector2(1f, 0f);
    }

    void RedrawHand()
    {
        foreach (var v in _manoViews) Destroy(v.gameObject);
        _manoViews.Clear();

        var hand = turn.Players[PlayerIndex].Hand;
        bool isMyTurn = turn.CurrentPlayerIndex == PlayerIndex;

        foreach (var t in hand)
        {
            bool canPlay = isMyTurn && (
                turn.Board.IsEmpty ||
                t.A == turn.Board.LeftValue || t.B == turn.Board.LeftValue ||
                t.A == turn.Board.RightValue || t.B == turn.Board.RightValue
            );

            var v = Instantiate(tileViewPrefab, panelMano);
            v.Setup(t, mainCanvas, OnTileDropped, canPlay);
            _manoViews.Add(v);
        }
    }

    void OnTileClicked(DominoTile tile)
    {
        if (turn.CurrentPlayerIndex != PlayerIndex) return;

        // ¿Puede ir a ambos lados?
        bool left = turn.Board.CanPlaceLeft(tile);
        bool right = turn.Board.CanPlaceRight(tile);

        if (left && right)
        {
            // Elección simple por ahora: preguntar por consola y usar derecha.
            // Para UI: mostramos dos botones temporales.
            ShowSideChooser(tile);
        }
        else if (right) turn.TryPlayRight(PlayerIndex, tile);
        else if (left)  turn.TryPlayLeft(PlayerIndex, tile);

        RedrawHand();
    }

    // Side chooser minimalista: usa dos botones emergentes temporales
    void ShowSideChooser(DominoTile tile)
    {
        txtMensaje.text = "Elegí lado: IZQ o DER";
        // por defecto uso derecha:
        turn.TryPlayRight(PlayerIndex, tile);
        txtMensaje.text = "";
    }

    void ShowEndPanel(int winnerIndex, string reason)
    {
        panelFinPartida.SetActive(true);

        if (winnerIndex == PlayerIndex)
        {
            txtResultado.text = "¡Ganaste!";
            txtRecompensa.text = "Has obtenido el tarro de miel";
            RewardSystemHook.Grant("TarroDeMiel");
        }
        else if (winnerIndex == -1)
        {
            txtResultado.text = "Empate";
            txtRecompensa.text = "No hay recompensa.";
        }
        else
        {
            txtResultado.text = "Perdiste";
            txtRecompensa.text = "Intenta de nuevo.";
        }

        btnReintentar.onClick.RemoveAllListeners();
        btnSalir.onClick.RemoveAllListeners();

        btnReintentar.onClick.AddListener(() =>
        {
            panelFinPartida.SetActive(false);
            turn.StartMatch(3);
            InitAndDraw();
        });

        btnSalir.onClick.AddListener(() =>
        {
            panelFinPartida.SetActive(false);
            SceneManager.LoadScene("Plaza");
        });
    }

    void OnTileDropped(DominoTile tile, Vector2 screenPos)
    {
        // Si no es tu turno, ignorar
        if (turn.CurrentPlayerIndex != PlayerIndex)
        {
            RedrawHand(); // vuelve todo a su sitio
            return;
        }

        // ¿Se soltó encima de la mesa?
        if (!RectTransformUtility.RectangleContainsScreenPoint(panelMesa, screenPos, mainCanvas.worldCamera))
        {
            // Fuera de la mesa → no se juega
            RedrawHand();
            return;
        }

        bool canLeft  = turn.Board.CanPlaceLeft(tile);
        bool canRight = turn.Board.CanPlaceRight(tile);

        // Calcular si soltó más hacia la izquierda o derecha de la mesa
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            panelMesa,
            screenPos,
            mainCanvas.worldCamera,
            out var localPos);

        bool dropLeftSide = localPos.x < 0f; // x < 0 = mitad izquierda

        bool toLeft;

        if (canLeft && canRight)
        {
            // Puede ir a ambos lados: usamos la mitad de la mesa
            toLeft = dropLeftSide;
        }
        else if (canLeft)
        {
            toLeft = true;
        }
        else if (canRight)
        {
            toLeft = false;
        }
        else
        {
            // Ningún lado válido → jugada inválida
            txtMensaje.text = "Jugada inválida";
            RedrawHand();
            return;
        }

        if (toLeft) turn.TryPlayLeft(PlayerIndex, tile);
        else        turn.TryPlayRight(PlayerIndex, tile);

        // Refrescar mesa y mano después de la jugada
        RedrawBoard();
        RedrawHand();
    }
}
