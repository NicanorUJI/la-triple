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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        panelInstrucciones.SetActive(true);
    }
    public void StartGame()
    {
        panelInstrucciones.SetActive(false);

        minijuegoController = FindObjectOfType<carta_controller>();
        minijuegoController.carta2Objeto.SetActive(false);
        minijuegoController.animator.gameObject.SetActive(false);
        carta1.tipo = 1; //esta carta es un as 
        carta2.tipo = 0; //esta carta esta vacia
        carta3.tipo = 2; //esta carta es un joker
        carta4.tipo = 1; //esta carta es un as
        minijuegoController.BarajarCartasAlcalde();


        turnoJuaquin = true;
    }

    public void MostrarPanelFin()
    {
        panelFinSolterona.SetActive(true);

        if (haGanado)
        {
            botonContinuar.interactable = true;
            imagenResultado.sprite = spriteVictoria;
            textoResultado.text = "Has guanyat a l'alcalde";
  
        }
        else
        {
            botonContinuar.interactable = false;
            imagenResultado.sprite = spriteDerrota;
            textoResultado.text = "Has perdut contra l'alcalde :(";
        }
    }

    public void ResetGame()
    {
        SceneManager.LoadScene("Minijuego_solterona");
    }
    public void FinishGame() 
    {
        GameManager.Change("Act2_Q_LLANCE_WonSolterona");
        Debug.Log("Cambiando a Act2_Q_LLANCE_WonSolterona");
        SceneManager.LoadScene("Bar");
    }
}
