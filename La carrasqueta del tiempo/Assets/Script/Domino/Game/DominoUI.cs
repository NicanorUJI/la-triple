using UnityEngine;
using UnityEngine.UI;
using Domino.Core;
using Domino.Game;
using Domino.UI;
using System.Collections.Generic;
using System.Linq;
using System.Collections; 
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

    [Header("Audio")]
    public AudioSource sourceMusica;      
    public AudioSource sourceSFX;         
    public AudioClip clipMusicaFondo;     
    public AudioClip clipColocarFicha;    
    public AudioClip clipVictoria; 
    public AudioClip clipDerrota;
    public AudioClip clipRobarPasar; // Usaremos este como sonido de interacción general

    readonly List<DominoTileView> _manoViews = new();
    readonly List<DominoTileView> _mesaViews = new();

    int PlayerIndex => 0; 

    [Header("Scroll Mesa")]
    public UnityEngine.UI.ScrollRect scrollMesa;
    public RectTransform contenidoMesa;

    [Header("Intro Reglas")]
    public GameObject panelInstruccionesDomino;
    public Button btnAcceptar; 
    private bool _gameStarted = false;

    void Awake()
    {
        btnRobar.onClick.AddListener(() =>
        {
            if (!_gameStarted) return;
            PlayDrawSound(); 
            turn.DrawOrPass(PlayerIndex);
        });

        btnPasar.onClick.AddListener(() =>
        {
            if (!_gameStarted) return;
            PlayDrawSound();
            turn.DrawOrPass(PlayerIndex);
        });

        if (btnAcceptar != null)
        {
            btnAcceptar.onClick.AddListener(() => 
            {
                StartCoroutine(StartGameSequence());
            });
        }
    }

    IEnumerator StartGameSequence()
    {
        if (btnAcceptar != null) btnAcceptar.interactable = false;

        if (sourceSFX != null && clipRobarPasar != null)
        {
            sourceSFX.PlayOneShot(clipRobarPasar);
            yield return new WaitForSeconds(clipRobarPasar.length);
        }
        else
        {
            yield return new WaitForSeconds(0.2f);
        }

        StartGame();
    }

    void PlayDrawSound()
    {
        if (sourceSFX != null && clipRobarPasar != null)
        {
            sourceSFX.PlayOneShot(clipRobarPasar);
        }
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
        RedrawHand(); 
        OnTurnChanged(turn.CurrentPlayerIndex);
    }

    public void ShowIntro()
    {
        _gameStarted = false;
        if (panelInstruccionesDomino != null)
            panelInstruccionesDomino.SetActive(true);
        
        if(btnAcceptar != null) btnAcceptar.interactable = true;
    }

    public void StartGame()
    {
        if (sourceMusica != null && clipMusicaFondo != null)
        {
            sourceMusica.clip = clipMusicaFondo;
            sourceMusica.loop = true; 
            sourceMusica.Play();
        }
        if (panelInstruccionesDomino != null)
            panelInstruccionesDomino.SetActive(false);

        _gameStarted = true;

        turn.StartMatch(3);
        InitAndDraw();
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
        RedrawHand(); 
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

    void PlayTileSound()
    {
        if (sourceSFX != null && clipColocarFicha != null)
        {
            sourceSFX.PlayOneShot(clipColocarFicha);
        }
    }

    void OnTileClicked(DominoTile tile)
    {
        if (turn.CurrentPlayerIndex != PlayerIndex) return;

        bool left = turn.Board.CanPlaceLeft(tile);
        bool right = turn.Board.CanPlaceRight(tile);

        if (left && right)
        {
            ShowSideChooser(tile);
        }
        else if (right) 
        {
            turn.TryPlayRight(PlayerIndex, tile);
            PlayTileSound(); 
        }
        else if (left)  
        {
            turn.TryPlayLeft(PlayerIndex, tile);
            PlayTileSound(); 
        }

        RedrawHand();
    }

    void ShowSideChooser(DominoTile tile)
    {
        txtMensaje.text = "Elegí lado: IZQ o DER";
        turn.TryPlayRight(PlayerIndex, tile);
        PlayTileSound(); 
        txtMensaje.text = "";
    }

    void ShowEndPanel(int winnerIndex, string reason)
    {
        sourceMusica.Stop();
        panelFinPartida.SetActive(true);

        bool playerWon = (winnerIndex == PlayerIndex);

        if (sourceSFX != null)
        {
            if (playerWon)
            {
                if (clipVictoria != null) sourceSFX.PlayOneShot(clipVictoria);
            }
            else
            {
                if (clipDerrota != null) sourceSFX.PlayOneShot(clipDerrota);
            }
        }

        btnSalir.gameObject.SetActive(playerWon);
        btnReintentar.gameObject.SetActive(!playerWon);
        
        btnSalir.interactable = playerWon;
        btnReintentar.interactable = true;

        if (winnerIndex == PlayerIndex)
        {
            txtResultado.text = "¡Ganaste!";
            txtRecompensa.text = "Has obtenido el tarro de miel";
            RewardSystemHook.Grant("TarroDeMiel");

            GameManager.Change("Act2_Q_MENJAR_WonDomino");
            
            var mc = MissionController.Instance ?? FindObjectOfType<MissionController>();
            if (mc != null)
            {
                mc.SetActiveMission(
                    3,
                    "Has guanyat el dominó",
                    "Has guanyat el pot de mel. Parla amb l'iaia al bar."
                );
            }
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

        // Listener Reintentar
        btnReintentar.onClick.RemoveAllListeners();
        btnReintentar.onClick.AddListener(() =>
        {
            StartCoroutine(RetryGameSequence());
        });

        // Listener Salir
        btnSalir.onClick.RemoveAllListeners();
        // +++ MODIFICADO: Llamamos a la Corrutina de Salir +++
        btnSalir.onClick.AddListener(() =>
        {
            StartCoroutine(ExitGameSequence());
        });
        // ++++++++++++++++++++++++++++++++++++++++++++++++++++
    }

    IEnumerator RetryGameSequence()
    {
        btnReintentar.interactable = false;

        if (sourceSFX != null && clipRobarPasar != null)
        {
            sourceSFX.PlayOneShot(clipRobarPasar);
            yield return new WaitForSeconds(clipRobarPasar.length);
        }
        else
        {
            yield return new WaitForSeconds(0.2f);
        }

        panelFinPartida.SetActive(false);
        _gameStarted = true;
        turn.StartMatch(3);
        InitAndDraw();
        sourceMusica.Play();
    }

    // +++ NUEVO: Corrutina para Salir esperando el audio +++
    IEnumerator ExitGameSequence()
    {
        // Desactivamos para que no le den click varias veces
        btnSalir.interactable = false;

        // Reproducimos el sonido y esperamos
        if (sourceSFX != null && clipRobarPasar != null)
        {
            sourceSFX.PlayOneShot(clipRobarPasar);
            yield return new WaitForSeconds(clipRobarPasar.length);
        }
        else
        {
            yield return new WaitForSeconds(0.2f);
        }

        // Ahora sí cambiamos de escena
        if (GameManager.Instance != null)
        {
            GameManager.Instance.lastExitName = "BarD"; 
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene("Bar");
    }
    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++

    void OnTileDropped(DominoTile tile, Vector2 screenPos)
    {
        if (!_gameStarted)
        {
            RedrawHand();
            return;
        }

        if (turn.CurrentPlayerIndex != PlayerIndex)
        {
            RedrawHand(); 
            return;
        }

        if (!RectTransformUtility.RectangleContainsScreenPoint(panelMesa, screenPos, mainCanvas.worldCamera))
        {
            RedrawHand();
            return;
        }

        bool canLeft  = turn.Board.CanPlaceLeft(tile);
        bool canRight = turn.Board.CanPlaceRight(tile);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            panelMesa,
            screenPos,
            mainCanvas.worldCamera,
            out var localPos);

        bool dropLeftSide = localPos.x < 0f; 

        bool toLeft;

        if (canLeft && canRight)
        {
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
            txtMensaje.text = "Jugada inválida";
            RedrawHand();
            return;
        }

        if (toLeft) turn.TryPlayLeft(PlayerIndex, tile);
        else        turn.TryPlayRight(PlayerIndex, tile);

        PlayTileSound(); 

        RedrawBoard();
        RedrawHand();
    }
}