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
    public TMP_Text debug_Text; //no hace falta
    public TMP_Text winner_Text;
    public GameObject winner_Object;
    public TMP_Text player_points_Text; //no hace falta
    public TMP_Text NPC_points_Text; //no hace falta
    public GameObject boton_Confirmar;


    [Header("Sliders")]
    public Slider slider_NumSacar;
    public GameObject object_NumSacar; //no hace falta

    public Slider slider_NumCantar;
    public GameObject object_NumCantar; //no hace falta

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
            //PRIMER CLICK: Se retira la UI de la primera fase
            fase_eleccionSacar.SetActive(false);
            fase_eleccionCantar.SetActive(true);

            //Se guarda el valor que ha sacado el jugador
            value_Sacar = slider_NumSacar.value;

            //Se retira la barra SACAR y se cambia por la de CANTAR
            object_NumSacar.SetActive(false);
            object_NumCantar.SetActive(true);
            valueText.SetText("2");
            endGame_nextClick=true;
        }
        else
        {
            //SEGUNDO CLICK: Se guarda el valor que ha sacado el jugador
            value_Cantar = slider_NumCantar.value;


            //Se retira la UI de la segunda fase
            fase_eleccionCantar.SetActive(false);
            burbuja_pensar.SetActive(false);
            object_NumCantar.SetActive(false);
            valueText.SetText("");


            //Eleccion numeros del NPC
            value_NPC_Sacar = morraController.sacar_NPC();
            value_NPC_Cantar = morraController.cantar_NPC(value_NPC_Sacar);

            debug_Text.SetText("El jugador saca: " + value_Sacar + 
                "\nEl jugador canta: " + value_Cantar + 
                "\nEl NPC saca: " + value_NPC_Sacar + 
                "\nEl NPC canta: " + value_NPC_Cantar);

            
            //INICIAR FASE DE FINAL DE RONDA
            StartCoroutine(fase_finalDeRonda());


        }
    }

    private void setPoints(bool player_win)
    {
        //Si gana el jugador; mirar puntos del 1 al 3
        if (player_win)
        {
            for (int i = 0; i < 3; i++)
            {
                SpriteRenderer img = puntos[i].GetComponent<SpriteRenderer>();

                if (img.sprite == sprite_noPoint) {
                    img.sprite = sprite_Point;
                    return;
                }

            }
        }
        //si gana el rival; mirar puntos el 4 al 6
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

        //Quitar UI fase final de ronda
        fase_finalRonda.SetActive(false);
        textos_cantar.SetActive(false);


        //UI de fase eleccion
        burbuja_pensar.SetActive(true);
        fase_eleccionSacar.SetActive(true);

        slider_NumSacar.value = 1;
        slider_NumCantar.value = 2;

        valueText.SetText("1");
    }

    

    private IEnumerator fase_finalDeRonda()
    {
        
        countdownObject.SetActive(true);

        for (int i = 0; i < 3; i++)
        {
            SpriteRenderer countdownImage = countdownObject.GetComponent<SpriteRenderer>();
            countdownImage.sprite = countdownSprites[i];


            yield return new WaitForSeconds(0.8f);
        }

        countdownObject.SetActive(false);

        //Ense�ar las manos y los bocadillos
        fase_finalRonda.SetActive(true);

        SpriteRenderer joaquinMano_spriteR = manoJoaquin.GetComponent<SpriteRenderer>();
        SpriteRenderer rivalMano_spriteR = manoRival.GetComponent<SpriteRenderer>();

        int value_sacarINT = (int)value_Sacar;

        joaquinMano_spriteR.sprite = manos_sprites[value_sacarINT-1];
        rivalMano_spriteR.sprite = manos_sprites[value_NPC_Sacar-1];

        //Ense�ar y poner texto del numero que cantan
        textos_cantar.SetActive(true);
        text_joaquinCantar.SetText(value_Cantar + " !");
        text_rivalCantar.SetText(value_NPC_Cantar + " !");


        setWinnerOfRound();

    }

    private void setWinnerOfRound()
    {
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
            setPoints(true);
        }

        else
        {
            winner_Text.SetText("Punto para el Alcalde");
            puntos_NPC = morraController.givePoint(false);
            setPoints(false);

        }


        //END GAME
        if (puntos_jugador >= 3 || puntos_NPC >= 3)
        {
            if (puntos_jugador > puntos_NPC)
            {
                GameManager.Change("Act2_Q_MENJAR_HasMeat");
                Debug.Log("[Menjar/Morra] Joaquín ha guanyat la cistella de menjar.");

                var mc = MissionController.Instance ?? FindObjectOfType<MissionController>();
                if (mc != null)
                {
                    mc.SetActiveMission(
                        3,
                        "Tornar amb la carn",
                        "Has guanyat una cistella de menjar a la Morra. Torna al passat amb Maripili."
                    );
                }

                SceneManager.LoadScene("PlazaPasado");
            }
            else
            {
                debug_Text.SetText("Ha ganado el alcalde");
                SceneManager.LoadScene("Minijuego_Morra");
            }
        }

        else
        {
            startNewRound_nextClick = true;
        }
    }
    
}
