using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    [SerializeField] private GameObject botonPausa;

    [SerializeField] private GameObject menuPausa;
    public void Pausa()
    {
        Time.timeScale = 0f;
        botonPausa.SetActive(false);
        menuPausa.SetActive(true);
    }

    public void Reanudar()
    {
        Time.timeScale = 1f;
        botonPausa.SetActive(true);
        menuPausa.SetActive(false);
    }

    public void Cerrar()
    {
        Debug.Log("Cerrando");
        Time.timeScale = 1f;
        //Application.Quit();
        SceneManager.LoadScene("Escena Menú"); ;
    }

    public void GuardarPartida()
    {
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
