using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class Button_Controller_Morra : MonoBehaviour
{
    [Header("Fase Eleccion")]
    public GameObject fase_eleccionSacar;
    public GameObject fase_eleccionCantar;
    public GameObject burbuja_pensar;

    [Header("UI puntos")]
    public GameObject[] puntos;
    public Sprite sprite_noPoint;
    public Sprite sprite_Point;

    [Header("Audio")]
    public AudioSource audioSource; 
    public AudioClip sonidoSacarMano; 
    public AudioClip sonidoCuentaAtras; 
    // --- NUEVO: Sonidos de fin de juego ---
    public AudioClip sonidoVictoria; 
    public AudioClip sonidoDerrota;
    public AudioClip sonidoClick;

    // --------------------------------------

    [Header("Cuenta atras")]
    public GameObject countdownObject;
    public Sprite[] countdownSprites;

    [Space(10)]

    [Header("Fase fin de ronda")]
    public GameObject fase_finalRonda;
    public GameObject manoJoaquin;
    public GameObject manoRival;
    public Sprite[] manos_sprites;

    public GameObject textos_cantar;
    public TMP_Text text_joaquinCantar;
    public TMP_Text text_rivalCantar;

    [Header("Texto y Boton")]
    public TMP_Text valueText;
    public TMP_Text debug_Text; 
    public TMP_Text winner_Text;
    public GameObject winner_Object;
    public TMP_Text player_points_Text; 
    public TMP_Text NPC_points_Text; 
    public GameObject boton_Confirmar;

    [Header("Sliders")]
    public Slider slider_NumSacar;
    public GameObject object_NumSacar; 

    public Slider slider_NumCantar;
    public GameObject object_NumCantar; 

    [Header("Scripts")]
    public Morra_Controller morraController;

    [Header("Panel Fin de Juego")]
    public GameObject panelFinMorra;
    public Image imagenResultado;
    public TMP_Text textoResultado;
    public Button botonContinuar;
    public Button botonReintentar;

    public Sprite spriteVictoria;
    public Sprite spriteDerrota;

    private bool haGanado = false;
    private bool endGame_nextClick = false;
    private bool startNewRound_nextClick = false;
    private float value_Sacar = 0;
    private float value_Cantar = 0;

    private int value_NPC_Sacar;
    private int value_NPC_Cantar;

    private int puntos_jugador = 0;
    private int puntos_NPC = 0;

    public void onClick()
    {
        audioSource.PlayOneShot(sonidoClick);

        if (startNewRound_nextClick)
        {
            startNewRound_nextClick = false;
            startNewRound();
        }
        else if (!endGame_nextClick && !startNewRound_nextClick)
        {
            //PRIMER CLICK
            fase_eleccionSacar.SetActive(false);
            fase_eleccionCantar.SetActive(true);

            value_Sacar = slider_NumSacar.value;

            object_NumSacar.SetActive(false);
            object_NumCantar.SetActive(true);
            valueText.SetText("2");
            endGame_nextClick = true;
        }
        else
        {
            //SEGUNDO CLICK
            value_Cantar = slider_NumCantar.value;

            fase_eleccionCantar.SetActive(false);
            burbuja_pensar.SetActive(false);
            object_NumCantar.SetActive(false);
            valueText.SetText("");

            value_NPC_Sacar = morraController.sacar_NPC();
            value_NPC_Cantar = morraController.cantar_NPC(value_NPC_Sacar);

            debug_Text.SetText("El jugador saca: " + value_Sacar +
                "\nEl jugador canta: " + value_Cantar +
                "\nEl NPC saca: " + value_NPC_Sacar +
                "\nEl NPC canta: " + value_NPC_Cantar);

            StartCoroutine(fase_finalDeRonda());
        }
    }

    private void setPoints(bool player_win)
    {
        if (player_win)
        {
            for (int i = 0; i < 3; i++)
            {
                SpriteRenderer img = puntos[i].GetComponent<SpriteRenderer>();
                if (img.sprite == sprite_noPoint)
                {
                    img.sprite = sprite_Point;
                    return;
                }
            }
        }
        else
        {
            for (int i = 3; i < 6; i++)
            {
                SpriteRenderer img = puntos[i].GetComponent<SpriteRenderer>();
                if (img.sprite == sprite_noPoint)
                {
                    img.sprite = sprite_Point;
                    return;
                }
            }
        }
    }

    private void startNewRound()
    {
        endGame_nextClick = false;
        winner_Object.SetActive(false);
        debug_Text.SetText(" ");
        object_NumCantar.SetActive(false);
        object_NumSacar.SetActive(true);
        fase_finalRonda.SetActive(false);
        textos_cantar.SetActive(false);
        burbuja_pensar.SetActive(true);
        fase_eleccionSacar.SetActive(true);
        slider_NumSacar.value = 1;
        slider_NumCantar.value = 2;
        valueText.SetText("1");
    }

    private void MostrarPanelFin()
    {
        if (morraController != null)
        {
            morraController.StopGame();
        }
        fase_eleccionSacar.SetActive(false);
        fase_eleccionCantar.SetActive(false);
        burbuja_pensar.SetActive(false);
        fase_finalRonda.SetActive(false);
        textos_cantar.SetActive(false);
        object_NumCantar.SetActive(false);
        object_NumSacar.SetActive(false);
        winner_Object.SetActive(false);
        countdownObject.SetActive(false);

        panelFinMorra.SetActive(true);

        if (haGanado)
        {
            imagenResultado.sprite = spriteVictoria;
            textoResultado.text = "Has guanyat contra l'alcalde";
            
            // --- NUEVO: SONIDO VICTORIA ---
            if(audioSource != null && sonidoVictoria != null)
            {
                audioSource.PlayOneShot(sonidoVictoria);
            }
        }
        else
        {
            imagenResultado.sprite = spriteDerrota;
            textoResultado.text = "Has perdut :(";

            // --- NUEVO: SONIDO DERROTA ---
            if (audioSource != null && sonidoDerrota != null)
            {
                audioSource.PlayOneShot(sonidoDerrota);
            }
        }

        if (botonContinuar != null)
            botonContinuar.interactable = haGanado;
    }

    private void setWinnerOfRound()
    {
        Button btnConfirmar = boton_Confirmar.GetComponent<Button>();
        btnConfirmar.interactable = true;

        int jugadorHaGanado = morraController.jugadorGanador(value_NPC_Sacar + value_Sacar, value_Cantar, value_NPC_Cantar);
        winner_Object.SetActive(true);

        if (jugadorHaGanado == 0)
        {
            winner_Text.SetText("Empate");
        }
        else if (jugadorHaGanado > 0)
        {
            winner_Text.SetText("Punt per a Joaquín");
            puntos_jugador = morraController.givePoint(true);
            setPoints(true);
        }
        else
        {
            winner_Text.SetText("Punt per a l'alcalde");
            puntos_NPC = morraController.givePoint(false);
            setPoints(false);
        }

        if (puntos_jugador >= 3 || puntos_NPC >= 3)
        {
            haGanado = puntos_jugador > puntos_NPC;
            btnConfirmar.interactable = false;
            StartCoroutine(MostrarPanelFinConDelay(2.5f));
        }
        else
        {
            startNewRound_nextClick = true;
        }
    }

    public void ContinuarJuego()
    {
        audioSource.PlayOneShot(sonidoClick);

        if (!haGanado) return;

        panelFinMorra.SetActive(false);
        var mc = MissionController.Instance ?? FindObjectOfType<MissionController>();
        if (mc != null)
        {
            mc.SetActiveMission(
                3,
                "Tornar amb la carn",
                "Has guanyat una cistella de menjar a la Morra. Torna al passat amb Maripili."
            );
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.lastExitName = "PlzM"; 
        }

        GameManager.Change("Act2_Q_MENJAR_HasMeat");
        SceneManager.LoadScene("PlazaPasado");
    }

    public void ReintentarJuego()
    {
        // 1. Reproducir sonido
        if (audioSource != null && sonidoClick != null)
        {
            audioSource.PlayOneShot(sonidoClick);
        }

        // 2. Iniciar la corrutina de espera (asegúrate de que el botón solo se pulse una vez)
        botonReintentar.interactable = false; // Opcional: evita doble click
        StartCoroutine(CargarEscenaConDelay());
    }

    // Esta es la corrutina que maneja el tiempo de espera
    private IEnumerator CargarEscenaConDelay()
    {
        // Espera 0.5 segundos (o el tiempo que dure tu clip de audio aproximadamente)
        yield return new WaitForSeconds(0.5f);

        // Carga la escena
        SceneManager.LoadScene("Minijuego_Morra");
    }

    private IEnumerator fase_finalDeRonda()
    {
        Button btnConfirmar = boton_Confirmar.GetComponent<Button>();
        btnConfirmar.interactable = false;

        countdownObject.SetActive(true);

        // Sonido cuenta atras (una vez)
        if (audioSource != null && sonidoCuentaAtras != null)
        {
            audioSource.PlayOneShot(sonidoCuentaAtras);
        }

        for (int i = 0; i < 3; i++)
        {
            SpriteRenderer countdownImage = countdownObject.GetComponent<SpriteRenderer>();
            countdownImage.sprite = countdownSprites[i];
            yield return new WaitForSeconds(0.8f);
        }

        countdownObject.SetActive(false);

        // Sonido al sacar la mano
        if (audioSource != null && sonidoSacarMano != null)
        {
            audioSource.PlayOneShot(sonidoSacarMano);
        }

        fase_finalRonda.SetActive(true);

        SpriteRenderer joaquinMano_spriteR = manoJoaquin.GetComponent<SpriteRenderer>();
        SpriteRenderer rivalMano_spriteR = manoRival.GetComponent<SpriteRenderer>();

        int value_sacarINT = (int)value_Sacar;

        joaquinMano_spriteR.sprite = manos_sprites[value_sacarINT - 1];
        rivalMano_spriteR.sprite = manos_sprites[value_NPC_Sacar - 1];

        textos_cantar.SetActive(true);
        text_joaquinCantar.SetText(value_Cantar + " !");
        text_rivalCantar.SetText(value_NPC_Cantar + " !");

        setWinnerOfRound();
    }

    private IEnumerator MostrarPanelFinConDelay(float delay)
    {
        yield return new WaitForSeconds(delay); 
        MostrarPanelFin(); 
    }
}