using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;


public class Button_Controller_Morra : MonoBehaviour
{
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


    private bool endGame_nextClick = false;
    private bool startNewRound_nextClick = false;
    private float value_Sacar = 0;
    private float value_Cantar = 0;

    private int value_NPC_Sacar;
    private int value_NPC_Cantar;

    private int puntos_jugador = 0;
    private int puntos_NPC = 0;

    ////////////////////////////////////////////////////

    public void onClick()
    {
        if (startNewRound_nextClick)
        {
            startNewRound_nextClick = false;
            startNewRound();
        }

        else if (!endGame_nextClick && !startNewRound_nextClick)
        {
            //eleccion numero a sacar del JUGADOR
            value_Sacar = slider_NumSacar.value;
            debug_Text.SetText("El jugador saca: " +  value_Sacar);

            object_NumSacar.SetActive(false);
            object_NumCantar.SetActive(true);
            valueText.SetText("2");
            endGame_nextClick=true;
        }
        else
        {
            //eleccion numero a cantar del JUGADOR
            value_Cantar = slider_NumCantar.value;
            debug_Text.SetText("El jugador saca: " + value_Sacar + "\nEl jugador canta: " + value_Cantar);

            object_NumCantar.SetActive(false);
            //boton_Confirmar.SetActive(false);
            valueText.SetText("");

            //Eleccion numeros del NPC
            value_NPC_Sacar = morraController.sacar_NPC();
            value_NPC_Cantar = morraController.cantar_NPC(value_NPC_Sacar);

            debug_Text.SetText("El jugador saca: " + value_Sacar + 
                "\nEl jugador canta: " + value_Cantar + 
                "\nEl NPC saca: " + value_NPC_Sacar + 
                "\nEl NPC canta: " + value_NPC_Cantar);

            //Eleccion ganador
            int jugadorHaGanado = morraController.jugadorGanador(value_NPC_Sacar + value_Sacar, value_Cantar, value_NPC_Cantar);
            winner_Object.SetActive(true);

            


            if (jugadorHaGanado == 0)
            {
                winner_Text.SetText("Empate");
            }

            else if (jugadorHaGanado > 0)
            {
                winner_Text.SetText("Punto para Joaquin");
                puntos_jugador = morraController.givePoint(true);
            }

            else
            {
                winner_Text.SetText("Punto para el Alcalde");
                puntos_NPC = morraController.givePoint(false);
                
            }

            player_points_Text.SetText("J: " + puntos_jugador);
            NPC_points_Text.SetText("A: " + puntos_NPC);


            //END GAME
            if (puntos_jugador >= 3 || puntos_NPC >= 3)
            {
                if (puntos_jugador>puntos_NPC)
                {
                    GameManager.Change("Act2_Q_MENJAR_HasMeat");
                    Debug.Log("[Menjar/Morra] Joaquín ha guanyat la cistella de menjar.");

                    var mc = MissionController.Instance ?? FindObjectOfType<MissionController>();
                    mc?.SetActiveMission(
                            3,
                            "Tornar amb la carn",
                            "Has guanyat una cistella de menjar a la Morra. Torna al passat amb Maripili."
                        );

                    SceneManager.LoadScene("PlazaPasado");
                }
                else
                {
                    debug_Text.SetText("Ha ganado el alcalde");
                }

                //END GAME ()
                
            }

            else
            {
                startNewRound_nextClick = true;
            }

        }
    }

    public void startNewRound()
    {
        endGame_nextClick = false;
        winner_Object.SetActive(false);

        debug_Text.SetText(" ");

        object_NumCantar.SetActive(false);
        object_NumSacar.SetActive(true);

        

        slider_NumSacar.value = 1;
        slider_NumCantar.value = 2;

        valueText.SetText("1");
    }

    /*public void endGame()
    {
        if (puntos_jugador >= 3) SceneManager.LoadScene(Plaza);

        else SceneManager.LoadScene(Escena Men�);
    }*/
}
