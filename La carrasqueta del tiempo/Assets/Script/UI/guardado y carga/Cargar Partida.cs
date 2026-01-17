using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadManager : MonoBehaviour
{
    [Header("Escena con objetos DontDestroyOnLoad")]
    public string escenaPersistente = "Plaza"; // La escena con objetos que necesitan DontDestroyOnLoad

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
        yield return null;

        // 1️⃣ Cargar la escena persistente primero (Additive)
        if (!string.IsNullOrEmpty(escenaPersistente))
        {
            AsyncOperation preload = SceneManager.LoadSceneAsync(escenaPersistente, LoadSceneMode.Additive);
            while (!preload.isDone)
                yield return null;

            Debug.Log("✅ Escena persistente cargada: " + escenaPersistente);
        }

        // 2️⃣ Cargar la escena guardada (Single)
        AsyncOperation mainScene = SceneManager.LoadSceneAsync(data.sceneName, LoadSceneMode.Single);
        while (!mainScene.isDone)
            yield return null;

        Debug.Log("✅ Escena guardada cargada: " + data.sceneName);

        // 3️⃣ Colocar jugador
        ColocarJugador(data);

        // 4️⃣ Restaurar audio
        PlayerPrefs.SetFloat("SavedAmbienteVolume", data.volumenAmbiente);
        PlayerPrefs.SetFloat("SavedSFXVolume", data.volumenEfectos);

        // 5️⃣ Esperar un par de frames para que los managers persistentes estén listos
        yield return null;
        yield return null;

        // 6️⃣ Restaurar rewards / flags
        RestaurarRewards(data);

        // 7️⃣ Reaplicar UI y estados visuales
        RewardManager.Instance?.ReaplicarRecompensas();

        Debug.Log("✅ Partida cargada completamente");
    }

    void ColocarJugador(GameData data)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = new Vector3(
                data.playerX,
                data.playerY,
                data.playerZ
            );
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró el jugador al cargar.");
        }
    }

    void RestaurarRewards(GameData data)
    {
        if (RewardManager.Instance == null)
        {
            Debug.LogWarning("⚠️ RewardManager no disponible.");
            return;
        }

        if (data.rewards == null)
        {
            Debug.Log("ℹ️ No hay rewards guardadas.");
            return;
        }

        RewardManager.Instance.rewards.Clear();

        foreach (string reward in data.rewards)
        {
            RewardManager.Instance.rewards.Add(reward);
            GameManager.Change(reward);
        }

        Debug.Log($"🔄 Rewards restauradas: {data.rewards.Count}");
    }
}
