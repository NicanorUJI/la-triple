using UnityEngine;

public class Morra_Controller : MonoBehaviour
{
    public GameObject panelInstrucciones;
    private bool gameStarted = false;

    private int NPC_sacar = 0;
    private int NPC_cantar = 0;

    private int player_points;
    private int NPC_points;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (panelInstrucciones != null)
            panelInstrucciones.SetActive(true);

        gameStarted = false;
    }
    public void StartGame()
    {
        if (panelInstrucciones != null)
            panelInstrucciones.SetActive(false);

        // Reset por si se vuelve a jugar
        player_points = 0;
        NPC_points = 0;

        gameStarted = true;
    }
    public int sacar_NPC()
    {
        System.Random rnd = new System.Random();
        NPC_sacar = rnd.Next(1, 6);  // creates a number between 1 and 12
        return NPC_sacar;
    }

    public int cantar_NPC(int numSacado)
    {
        System.Random rnd = new System.Random();
        NPC_cantar = rnd.Next(numSacado+1, 11);  // creates a number between 1 and 12
        return NPC_cantar;
    }

    public int jugadorGanador(float total, float jugador_cantar, float NPC_cantar)
    {
        float diferenciaJugador = Mathf.Abs(jugador_cantar - total);
        Debug.Log("Num jugador: " + jugador_cantar);

        Debug.Log("Diferencia con el total: " + diferenciaJugador);



        float diferenciaNPC = Mathf.Abs(NPC_cantar - total);

        if (diferenciaJugador == diferenciaNPC) return 0;
        else if (diferenciaJugador < diferenciaNPC) return 1;
        else return -1;
    }

    public int givePoint(bool jugadorGanador)
    {
        if(jugadorGanador)
        {
            return ++player_points;
        }
        else
        {
            return ++NPC_points;
        }
    }

    
}
