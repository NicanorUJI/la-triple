using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class carta_controller : MonoBehaviour
{
    public Animator animator;

    [Header("Cartas rival")]
    public carta thisCarta; // carta que clickas
    public carta otherCarta; // carta que no clickas

    [Header("Cartas jugador")]
    public carta carta1;
    public carta carta2;

    [Header("OBJETOS Cartas rival")]
    public GameObject thisCartaObjeto;
    public GameObject otherCartaObjeto;

    [Header("Cartas jugador")]
    public GameObject carta1Objeto;
    public GameObject carta2Objeto;

    [Header("UI y Textos")]
    public GameObject texto_victoria;
    public GameObject texto_derrota;
    public Sprite[] imagenesPorTipo;
    public GameObject continuar;
    public GameObject reset;

    // --- NUEVO: Variables de Audio ---
    [Header("Audio")]
    public AudioSource audioSource; // Arrastra aquí el componente AudioSource
    public AudioClip sonidoClick;   // Arrastra aquí el archivo de sonido
    // --------------------------------

    private SolteronaGeneral minijuegoController;

    public void onClick()
    {
        minijuegoController = FindObjectOfType<SolteronaGeneral>();
        Debug.Log("minijuegoController = " + minijuegoController);
        Debug.Log("turnoJuaquin = " + minijuegoController.turnoJuaquin);

        if (minijuegoController.turnoJuaquin)
        {
            // --- NUEVO: Reproducir sonido ---
            if (audioSource != null && sonidoClick != null)
            {
                // Usamos PlayOneShot para que no se corte si hay otros sonidos
                audioSource.PlayOneShot(sonidoClick);
            }
            // --------------------------------

            StartCoroutine(jugadorEligeCarta());
            Debug.Log(">>> CORUTINA jugadorEligeCarta INICIADA");
        }        
    }

    IEnumerator jugadorEligeCarta()
    {
        //cambiar tipo de la carta vacia de la mano del jugador
        if (carta1.tipo == 0) //la carta está vacia
        {
            carta1.tipo = thisCarta.tipo;
            carta1Objeto.SetActive(true);
            ActualizarImagen(carta1Objeto, carta1);
        }

        else if (carta2.tipo == 0)
        {
            carta2.tipo = thisCarta.tipo;
            carta2Objeto.SetActive(true);
            ActualizarImagen(carta2Objeto, carta2);
        }

        BarajarCartasJugador();

        thisCarta.tipo = 0;

        minijuegoController.turnoJuaquin = false;
        GetComponent<Image>().enabled = false;
        yield return new WaitForSeconds(2.0f);


        if (carta1.tipo == 1 && carta2.tipo == 1)
        {
            carta1Objeto.SetActive(false);
            carta2Objeto.SetActive(false);
            thisCartaObjeto.SetActive(false);
            otherCartaObjeto.SetActive(false);

            minijuegoController.haGanado = true;
            minijuegoController.MostrarPanelFin();
        }
  
        alcaldeEligeCarta();
    }


    void alcaldeEligeCarta()
    {
        Debug.Log(">>> alcaldeEligeCarta EJECUTADA");
        minijuegoController = FindObjectOfType<SolteronaGeneral>();
        Debug.Log("turnoJuaquin = " + minijuegoController.turnoJuaquin);

       //Eleccion de que carta robar
        int numero = Random.Range(1, 3);
        Debug.Log("numero = " + numero);

        if (numero==1)
        {
            carta1Objeto.SetActive(false); ;
            thisCarta.tipo = carta1.tipo;
            carta1.tipo = 0;
        }
        else
        {
            carta2Objeto.SetActive(false);
            thisCarta.tipo = carta2.tipo;
            carta2.tipo = 0;
        }

        BarajarCartasAlcalde();

        minijuegoController.turnoJuaquin = true;
        GetComponent<Image>().enabled = true;
       
        Debug.Log("turnoJuaquin = " + minijuegoController.turnoJuaquin);

        if (minijuegoController.carta3.tipo == 1 && minijuegoController.carta4.tipo == 1) 
        {
            carta1Objeto.SetActive(false);
            carta2Objeto.SetActive(false);
            thisCartaObjeto.SetActive(false);
            otherCartaObjeto.SetActive(false);

            minijuegoController.haGanado = false;
            minijuegoController.MostrarPanelFin();
        }
        
        // NOTA IMPORTANTE: jugadorEligeCarta es una Corutina.
        // Si la llamas así, no hará nada. Deberías llamarla con StartCoroutine si esa era tu intención.
        // Pero cuidado, esto podría crear un bucle infinito si el turno es automático.
        // jugadorEligeCarta(); 
    }


    public void ActualizarImagen(GameObject objetoCarta, carta cartaActual)
    {
        Image img = objetoCarta.GetComponent<Image>();
        img.sprite = imagenesPorTipo[cartaActual.tipo];
    }

    public void BarajarCartasJugador()
    {
        if (carta1.tipo != 0 && carta2.tipo != 0)
        {
            if (Random.Range(0, 2) == 1)
            {
                int tipoTemporal = carta1.tipo;
                carta1.tipo = carta2.tipo;
                carta2.tipo = tipoTemporal;

                ActualizarImagen(carta1Objeto, carta1);
                ActualizarImagen(carta2Objeto, carta2);

                Debug.Log(">>> Cartas del jugador barajadas");
            }
        }
    }

    public void BarajarCartasAlcalde()
    {
        if (thisCarta.tipo != 0 && otherCarta.tipo != 0)
        {
            animator.gameObject.SetActive(false);
            animator.SetTrigger("mezclar_A");
            if (Random.Range(0, 2) == 1)
            {
                int tipoTemporal = thisCarta.tipo;
                thisCarta.tipo =   otherCarta.tipo;
                otherCarta.tipo = tipoTemporal;

                Debug.Log(">>> Cartas del jugador barajadas");
            }
        }
        Debug.Log(">>> Cartas barajadas");
    }
}