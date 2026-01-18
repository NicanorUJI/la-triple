using UnityEngine;

public class Morra_Controller : MonoBehaviour
{
    public GameObject panelInstrucciones;
    private bool gameStarted = false;

    private int NPC_sacar = 0;
    private int NPC_cantar = 0;

    private int player_points;
    private int NPC_points;

    // +++ NUEVO: Variable para la música +++
    [Header("Audio")]
    public AudioSource musicaFondo;
    // ++++++++++++++++++++++++++++++++++++++

    void Start()
    {
        if (panelInstrucciones != null)
            panelInstrucciones.SetActive(true);

        gameStarted = false;

        // +++ NUEVO: Asegurarnos de que no suene al principio +++
        if (musicaFondo != null)
        {
            musicaFondo.Stop();
        }
    }

    public void StartGame()
    {
        if (panelInstrucciones != null)
            panelInstrucciones.SetActive(false);

        // Reset por si se vuelve a jugar
        player_points = 0;
        NPC_points = 0;

        gameStarted = true;

        // +++ NUEVO: Iniciar la música al empezar el juego +++
        if (musicaFondo != null)
        {
            musicaFondo.Play();
        }
    }

    public int sacar_NPC()
    {
        System.Random rnd = new System.Random();
        NPC_sacar = rnd.Next(1, 6);  // creates a number between 1 and 6 (excluye el limite superior en Next integer)
        return NPC_sacar;
    }

    public int cantar_NPC(int numSacado)
    {
        System.Random rnd = new System.Random();
        // Nota: En System.Random.Next(min, max), max es exclusivo.
        // Si quieres hasta 10, pon 11.
        NPC_cantar = rnd.Next(numSacado + 1, 11); 
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
        // Nota: Aquí se suman puntos, pero no veo la condición de fin de juego.
        // He añadido la lógica de parar música si quisieras detenerla manualmente.
        
        if (jugadorGanador)
        {
            return ++player_points;
        }
        else
        {
            return ++NPC_points;
        }
    }

    // +++ NUEVO: Método para detener el juego y la música +++
    // Llama a este método cuando alguien llegue a X puntos (Game Over)
    public void StopGame()
    {
        gameStarted = false;
        if (musicaFondo != null)
        {
            musicaFondo.Stop();
        }
    }
}