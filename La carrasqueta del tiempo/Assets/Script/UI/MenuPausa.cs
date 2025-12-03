using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    [SerializeField] private GameObject botonPausa;
    [SerializeField] private GameObject menuPausa;
    [SerializeField] private AudioSource sonidoBotonPausa;
    [SerializeField] private AudioSource sonidoBotonOpcion;


    public void Pausa()
    {
        if (sonidoBotonPausa != null)
            AudioManager.instance.Reproducir(AudioManager.instance.botonPausaClip);  // 🔊 sonido de clic
        Time.timeScale = 0f;
        botonPausa.SetActive(false);
        menuPausa.SetActive(true);

        if (MusicaAmbiente.instance != null)
            MusicaAmbiente.instance.PausarMusica();
    }

    public void Reanudar()
    {
        if (sonidoBotonOpcion != null)
            AudioManager.instance.Reproducir(AudioManager.instance.botonOpcionClip);  // 🔊 sonido de clic
        Time.timeScale = 1f;
        botonPausa.SetActive(true);
        menuPausa.SetActive(false);

        if (MusicaAmbiente.instance != null)
            MusicaAmbiente.instance.ReanudarMusica();
    }

    public void Cerrar()
    {
        if (sonidoBotonOpcion != null)
            AudioManager.instance.Reproducir(AudioManager.instance.botonOpcionClip);  // 🔊 sonido de clic
        Debug.Log("Cerrando");
        Time.timeScale = 1f;
        if (MusicaAmbiente.instance != null)
        {
             Destroy(MusicaAmbiente.instance.gameObject);
        }
        SceneManager.LoadScene("Escena Menú");
    }

    public void GuardarPartida()
    {
        if (sonidoBotonOpcion != null)
            AudioManager.instance.Reproducir(AudioManager.instance.botonOpcionClip);  // 🔊 sonido de clic
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("No se encontró el jugador para guardar.");
            return;
        }

        GameData data = new GameData();
        data.playerX = player.transform.position.x;
        data.playerY = player.transform.position.y;
        data.playerZ = player.transform.position.z;
        data.sceneName = SceneManager.GetActiveScene().name;

        SaveSystem.Save(data);
        Debug.Log("Partida guardada.");
    }
}
