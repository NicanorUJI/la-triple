using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadManager : MonoBehaviour
{
    public void CargarPartida()
    {
        GameData data = SaveSystem.Load();

        if (data == null)
        {
            Debug.Log("⚠️ No hay partida guardada disponible.");
            return;
        }

        StartCoroutine(CargarEscena(data));
    }

    IEnumerator CargarEscena(GameData data)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(data.sceneName);
        while (!async.isDone)
            yield return null;

        // 1️⃣ Colocar jugador
        ColocarJugador(data);

        // 2️⃣ Restaurar audio
        if (data.volumenAmbiente > 0)
            PlayerPrefs.SetFloat("SavedAmbienteVolume", data.volumenAmbiente);

        if (data.volumenEfectos > 0)
            PlayerPrefs.SetFloat("SavedSFXVolume", data.volumenEfectos);

        // 3️⃣ Esperar a que la UI exista
        yield return null;
        yield return null;

        // 4️⃣ Refrescar UI de recompensas (seguro)
        if (RewardManager.Instance != null)
        {
            RewardManager.Instance.ReaplicarRecompensas();
        }

        Debug.Log("✅ Partida cargada completamente");
    }

    void ColocarJugador(GameData data)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position =
                new Vector3(data.playerX, data.playerY, data.playerZ);
        }
    }
}
