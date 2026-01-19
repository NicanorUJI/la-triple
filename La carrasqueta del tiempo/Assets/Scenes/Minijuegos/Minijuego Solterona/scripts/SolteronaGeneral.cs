using System.Collections;
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
    public AudioSource sourceMusica;      
    public AudioSource sourceSFX;         
    public AudioClip clipMusicaFondo;     
    public AudioClip clipVictoria;        
    public AudioClip clipDerrota;         
    public AudioClip clipSeleccionarCarta;
    // ++++++++++++++++++++++++

    void Start()
    {
        panelInstrucciones.SetActive(true);
    }

    public void StartGame()
    {
        sourceSFX.PlayOneShot(clipSeleccionarCarta);
        panelInstrucciones.SetActive(false);

        if (sourceMusica != null && clipMusicaFondo != null)
        {
            sourceMusica.clip = clipMusicaFondo;
            sourceMusica.loop = true; 
            sourceMusica.Play();
        }

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

    public void ReproducirSonidoCarta()
    {
        if (sourceSFX != null && clipSeleccionarCarta != null)
        {
            sourceSFX.PlayOneShot(clipSeleccionarCarta);
        }
    }

    public void MostrarPanelFin()
    {
        if (sourceMusica != null) sourceMusica.Stop();

        panelFinSolterona.SetActive(true);

        if (haGanado)
        {
            botonContinuar.interactable = true;
            imagenResultado.sprite = spriteVictoria;
            textoResultado.text = "Has guanyat a l'alcalde";

            if (sourceSFX != null && clipVictoria != null)
                sourceSFX.PlayOneShot(clipVictoria);
        }
        else
        {
            botonContinuar.interactable = false;
            imagenResultado.sprite = spriteDerrota;
            textoResultado.text = "Has perdut contra l'alcalde :(";

            if (sourceSFX != null && clipDerrota != null)
                sourceSFX.PlayOneShot(clipDerrota);
        }
    }

    // =========================================================
    // BOTÓN REINTENTAR (RESET)
    // =========================================================
    public void ResetGame()
    {
        StartCoroutine(ResetGameSequence());
    }

    private IEnumerator ResetGameSequence()
    {
        if (sourceSFX != null && clipSeleccionarCarta != null)
        {
            sourceSFX.PlayOneShot(clipSeleccionarCarta);
            yield return new WaitForSeconds(clipSeleccionarCarta.length);
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
        }

        SceneManager.LoadScene("Minijuego_solterona");
    }

    // =========================================================
    // BOTÓN CONTINUAR (FINISH) - MODIFICADO
    // =========================================================
    public void FinishGame()
    {
        // Iniciamos la secuencia de finalización
        StartCoroutine(FinishGameSequence());
    }

    private IEnumerator FinishGameSequence()
    {
        // 1. Sonido y Espera
        if (sourceSFX != null && clipSeleccionarCarta != null)
        {
            sourceSFX.PlayOneShot(clipSeleccionarCarta);
            yield return new WaitForSeconds(clipSeleccionarCarta.length);
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
        }

        // 2. Lógica del GameManager y Cambio de Escena
        if (GameManager.Instance != null)
        {
            GameManager.Instance.lastExitName = "BarM"; 
        }

        GameManager.Change("Act2_Q_LLANCE_WonSolterona");
        Debug.Log("Cambiando a Act2_Q_LLANCE_WonSolterona");

        SceneManager.LoadScene("Bar");
    }
}