using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


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

    private SolteronaGeneral minijuegoController;
    public GameObject texto_victoria;
    public GameObject texto_derrota;
    public Sprite[] imagenesPorTipo;
    public GameObject continuar;
    public GameObject reset;




    public void onClick()
    {
        minijuegoController = FindObjectOfType<SolteronaGeneral>();
        Debug.Log("minijuegoController = " + minijuegoController);
        Debug.Log("turnoJuaquin = " + minijuegoController.turnoJuaquin);

        if (minijuegoController.turnoJuaquin)
        {
            StartCoroutine(jugadorEligeCarta());
            Debug.Log(">>> CORUTINA jugadorEligeCarta INICIADA");
            //espera un segundo
            //alcaldeEligeCarta();
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


        if (carta1.tipo == 1 && carta2.tipo ==1)
        
        {
            carta1Objeto.SetActive(false);
            carta2Objeto.SetActive(false);
            thisCartaObjeto.SetActive(false);
            otherCartaObjeto.SetActive(false);
            texto_victoria.SetActive(true);
            reset.SetActive(true);
            continuar.SetActive(true);
        }

        //thisCartaObjeto.SetActive(false);
  
        alcaldeEligeCarta();
    }


    void alcaldeEligeCarta()
    {

       
        Debug.Log(">>> alcaldeEligeCarta EJECUTADA");
        minijuegoController = FindObjectOfType<SolteronaGeneral>();
        Debug.Log("turnoJuaquin = " + minijuegoController.turnoJuaquin);

        //carta1Objeto.SetActive(false);

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

        //thisCarta.tipo = numero;

        minijuegoController.turnoJuaquin = true;
        GetComponent<Image>().enabled = true;

       
        Debug.Log("turnoJuaquin = " + minijuegoController.turnoJuaquin);

        if (minijuegoController.carta3.tipo == 1 && minijuegoController.carta4.tipo == 1) 
        {
            carta1Objeto.SetActive(false);
            carta2Objeto.SetActive(false);
            thisCartaObjeto.SetActive(false);
            otherCartaObjeto.SetActive(false);
            texto_derrota.SetActive(true);
            reset.SetActive(true);
        
        }

       
        jugadorEligeCarta();




    }

    public void ActualizarImagen(GameObject objetoCarta, carta cartaActual)
    {
        Image img = objetoCarta.GetComponent<Image>();
        img.sprite = imagenesPorTipo[cartaActual.tipo];
    }

    public void BarajarCartasJugador()
    {
        // Si las dos cartas tienen un tipo asignado (no vacías)
        if (carta1.tipo != 0 && carta2.tipo != 0)
        {
            // 50% probabilidad de intercambiar
            if (Random.Range(0, 2) == 1)
            {
                int tipoTemporal = carta1.tipo;
                carta1.tipo = carta2.tipo;
                carta2.tipo = tipoTemporal;

                // Actualizar imágenes después de barajar
                ActualizarImagen(carta1Objeto, carta1);
                ActualizarImagen(carta2Objeto, carta2);

                Debug.Log(">>> Cartas del jugador barajadas");
            }
        }
    }

    public void BarajarCartasAlcalde()
    {
        // Si las dos cartas tienen un tipo asignado (no vacías)
        if (thisCarta.tipo != 0 && otherCarta.tipo != 0)
        {
            animator.gameObject.SetActive(false);
            animator.SetTrigger("mezclar_A");
            // 50% probabilidad de intercambiar
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

    public void Reset()
    {
        SceneManager.LoadScene("Minijuego_solterona");
    }

    public void Continuar()
    {
        SceneManager.LoadScene("Plaza");
    }

}
