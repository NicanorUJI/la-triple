using UnityEngine;
// No necesitamos listas ni SceneManagement para este enfoque, el código queda más limpio.

public class Morra_Controller : MonoBehaviour
{
    public GameObject panelInstrucciones;
    private bool gameStarted = false;

    private int NPC_sacar = 0;
    private int NPC_cantar = 0;

    private int player_points;
    private int NPC_points;

    [Header("Audio")]
    public AudioSource musicaFondo; // La música propia del minijuego
    public AudioSource sfxSource;   
    public AudioClip sonidoBoton;   

    void Start()
    {
        if (panelInstrucciones != null)
            panelInstrucciones.SetActive(true);

        gameStarted = false;

        if (musicaFondo != null)
        {
            musicaFondo.Stop();
        }
    }

    public void StartGame()
    {
        // 1. Sonido del botón
        if (sfxSource != null && sonidoBoton != null)
        {
            sfxSource.PlayOneShot(sonidoBoton);
        }

        // --- MODIFICACIÓN INICIO ---
        // Al empezar el juego, pausamos la música global (igual que en AutoBeatDetector)
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PausarMusica();
        }
        // --- MODIFICACIÓN FIN ---

        if (panelInstrucciones != null)
            panelInstrucciones.SetActive(false);

        // Reset por si se vuelve a jugar
        player_points = 0;
        NPC_points = 0;

        gameStarted = true;

        // Iniciamos la música propia del minijuego
        if (musicaFondo != null)
        {
            musicaFondo.Play();
        }
    }

    public int sacar_NPC()
    {
        System.Random rnd = new System.Random();
        NPC_sacar = rnd.Next(1, 6);  
        return NPC_sacar;
    }

    public int cantar_NPC(int numSacado)
    {
        System.Random rnd = new System.Random();
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
        if (jugadorGanador)
        {
            return ++player_points;
        }
        else
        {
            return ++NPC_points;
        }
    }

    public void StopGame()
    {
        gameStarted = false;

        // Detenemos la música específica de la Morra
        if (musicaFondo != null)
        {
            musicaFondo.Stop();
        }

        // --- MODIFICACIÓN INICIO ---
        // Al terminar el minijuego, reanudamos la música global
        if (AudioManager.instance != null)
        {
            AudioManager.instance.ReanudarMusica();
        }
        // --- MODIFICACIÓN FIN ---
    }
}