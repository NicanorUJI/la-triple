using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SolteronaGeneral : MonoBehaviour
{
    public GameObject panelInstrucciones;

    [Header("Cartas rival")]
    public carta carta3;
    public carta carta4;

    [Header("Cartas jugador")]
    public carta carta1;
    public carta carta2;

    public bool turnoJuaquin;
    private carta_controller minijuegoController;

    [Header("Panel Fin de Juego")]
    public GameObject panelFinSolterona;
    public Image imagenResultado;
    public TMP_Text textoResultado;
    public Button botonContinuar;
    public Button botonReintentar;
    public Sprite spriteVictoria;
    public Sprite spriteDerrota;

    public bool haGanado = false;

    // +++ SECCIÓN DE AUDIO +++
    [Header("Audio")]
    public AudioSource sourceMusica;      // Arrastra aquí el AudioSource para música
    public AudioSource sourceSFX;         // Arrastra aquí el AudioSource para efectos
    public AudioClip clipMusicaFondo;     // Música en bucle
    public AudioClip clipVictoria;        // Sonido ganar
    public AudioClip clipDerrota;         // Sonido perder
    public AudioClip clipSeleccionarCarta;// Sonido al tocar una carta
    // ++++++++++++++++++++++++

    void Start()
    {
        panelInstrucciones.SetActive(true);
    }

    public void StartGame()
    {
        panelInstrucciones.SetActive(false);

        // +++ INICIAR MÚSICA +++
        if (sourceMusica != null && clipMusicaFondo != null)
        {
            sourceMusica.clip = clipMusicaFondo;
            sourceMusica.loop = true; // Importante para que no pare
            sourceMusica.Play();
        }
        // ++++++++++++++++++++++

        minijuegoController = FindObjectOfType<carta_controller>();
        minijuegoController.carta2Objeto.SetActive(false);
        minijuegoController.animator.gameObject.SetActive(false);
        carta1.tipo = 1; 
        carta2.tipo = 0; 
        carta3.tipo = 2; 
        carta4.tipo = 1; 
        minijuegoController.BarajarCartasAlcalde();

        turnoJuaquin = true;
    }

    // +++ NUEVA FUNCIÓN PÚBLICA PARA SONIDO DE CARTA +++
    // Esta función debe ser llamada desde tu script "carta.cs" o desde el botón de la carta
    public void ReproducirSonidoCarta()
    {
        if (sourceSFX != null && clipSeleccionarCarta != null)
        {
            sourceSFX.PlayOneShot(clipSeleccionarCarta);
        }
    }
    // ++++++++++++++++++++++++++++++++++++++++++++++++++

    public void MostrarPanelFin()
    {
        // +++ DETENER MÚSICA AL TERMINAR +++
        if (sourceMusica != null) sourceMusica.Stop();
        // +++++++++++++++++++++++++++++++++

        panelFinSolterona.SetActive(true);

        if (haGanado)
        {
            botonContinuar.interactable = true;
            imagenResultado.sprite = spriteVictoria;
            textoResultado.text = "Has guanyat a l'alcalde";

            // +++ SONIDO VICTORIA +++
            if (sourceSFX != null && clipVictoria != null)
                sourceSFX.PlayOneShot(clipVictoria);
        }
        else
        {
            botonContinuar.interactable = false;
            imagenResultado.sprite = spriteDerrota;
            textoResultado.text = "Has perdut contra l'alcalde :(";

            // +++ SONIDO DERROTA +++
            if (sourceSFX != null && clipDerrota != null)
                sourceSFX.PlayOneShot(clipDerrota);
        }
    }

    public void ResetGame()
    {
        SceneManager.LoadScene("Minijuego_solterona");
    }

    public void FinishGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.lastExitName = "BarM"; 
        }

        GameManager.Change("Act2_Q_LLANCE_WonSolterona");
        Debug.Log("Cambiando a Act2_Q_LLANCE_WonSolterona");

        SceneManager.LoadScene("Bar");
    }
}