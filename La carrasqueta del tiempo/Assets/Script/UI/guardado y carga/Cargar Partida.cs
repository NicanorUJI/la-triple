using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

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
        // 1️⃣ Cargar escena
        AsyncOperation async = SceneManager.LoadSceneAsync(data.sceneName);
        while (!async.isDone)
            yield return null;

        // 2️⃣ Colocar jugador
        ColocarJugador(data);

        // 3️⃣ Restaurar audio
        PlayerPrefs.SetFloat("SavedAmbienteVolume", data.volumenAmbiente);
        PlayerPrefs.SetFloat("SavedSFXVolume", data.volumenEfectos);

        // 4️⃣ Esperar a que managers persistentes estén listos
        yield return null;
        yield return null;

        // 5️⃣ Restaurar rewards / flags
        RestaurarRewards(data);

        // 6️⃣ Reaplicar UI y estados visuales
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

        // Limpiar estado actual
        RewardManager.Instance.rewards.Clear();

        // Restaurar rewards y flags internas
        foreach (string reward in data.rewards)
        {
            RewardManager.Instance.rewards.Add(reward);

            // 🔹 Restaurar flags SIN ejecutar lógica secundaria
            GameManager.Change(reward);
        }

        Debug.Log($"🔄 Rewards restauradas: {data.rewards.Count}");
    }
}
