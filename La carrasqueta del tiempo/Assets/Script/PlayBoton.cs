using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButtonUI : MonoBehaviour
{
    public void PlayGame()
    {
        // 1️⃣ Borrar archivo de guardado
        SaveSystem.DeleteSave();

        // 2️⃣ Resetear estado en memoria
        GameManager.ResetGame();

        if (RewardManager.Instance != null)
            RewardManager.Instance.ResetUI();

        // 3️⃣ Cargar escena inicial
        SceneManager.LoadScene("Plaza");
    }
}
