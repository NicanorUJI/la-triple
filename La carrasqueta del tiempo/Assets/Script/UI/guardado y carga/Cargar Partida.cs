using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadManager : MonoBehaviour
{
    // 🔹 Este método lo puedes asignar al botón de "Cargar Partida"
    public void CargarPartida()
    {
        GameData data = SaveSystem.Load();

        if (data == null)
        {
            Debug.Log("⚠️ No hay partida guardada disponible.");
            return;
        }

        // Si estamos en el menú, cargar la escena guardada
        if (SceneManager.GetActiveScene().name == "Escena Menú")
        {
            Debug.Log("Cargando escena guardada...");
            StartCoroutine(CargarEscena(data));
        }
        else
        {
            // Si ya estás dentro del juego, simplemente recoloca al jugador
            if (SceneManager.GetActiveScene().name == data.sceneName)
            {
                ColocarJugador(data);
            }
            else
            {
                StartCoroutine(CargarEscena(data));
            }
        }
    }

    IEnumerator CargarEscena(GameData data)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(data.sceneName);
        while (!async.isDone)
            yield return null;

        ColocarJugador(data);
    }

    void ColocarJugador(GameData data)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = new Vector3(data.playerX, data.playerY, data.playerZ);
            Debug.Log("Jugador colocado en la posición guardada.");
        }
    }
}
