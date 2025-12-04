using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class carta_controller : MonoBehaviour
{
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
        }

        else if (carta2.tipo == 0)
        {
            carta2.tipo = thisCarta.tipo;
            carta2Objeto.SetActive(true);
        }

        thisCarta.tipo = 0;

        minijuegoController.turnoJuaquin = false;
        GetComponent<Image>().enabled = false;
        yield return new WaitForSeconds(1.0f);


        if (carta1.tipo == 1 && carta2.tipo ==1)
        
        {
            carta1Objeto.SetActive(false);
            carta2Objeto.SetActive(false);
            thisCartaObjeto.SetActive(false);
            otherCartaObjeto.SetActive(false);
            texto_victoria.SetActive(true);
        }

        //thisCartaObjeto.SetActive(false);
        //barajar_joaquin();
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
        
        

        thisCarta.tipo = numero;

       
       
       
        

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
        
        }

        //barajar_alcalde();
        jugadorEligeCarta();




    }

    void barajar_joaquin()
    {
        int random  = Random.Range(1, 3);
        Debug.Log("random = " + random);
        if (random == 2) 
        
        {
            carta1.tipo = carta2.tipo;
            carta2.tipo = carta1.tipo;
        }
       
    }

    void barajar_alcalde()
    {
        int random = Random.Range(1, 3);
        Debug.Log("random = " + random);
        if (random == 2)
        {
            thisCarta.tipo = otherCarta.tipo;
            otherCarta.tipo = thisCarta.tipo;
        }
           
    }


}
