using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayButtonUI : MonoBehaviour
{
    [Header("UI Confirmación")]
    [SerializeField] private GameObject panelConfirmacionNuevaPartida;
    [SerializeField] private Button botonCargarPartida;

    private Image imagenBotonCargar;
    void Awake()
    {
        imagenBotonCargar = botonCargarPartida.GetComponent<Image>();

        ActualizarColorBotonCargar();
    }

    void Start()
    {
        if (panelConfirmacionNuevaPartida != null)
            panelConfirmacionNuevaPartida.SetActive(false);
    }

    void ActualizarColorBotonCargar()
    {
        bool hayGuardado = SaveSystem.ExisteGuardado();

        if (!hayGuardado)
        {
            // 🔹 Botón oscuro
            imagenBotonCargar.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }
        else
        {
            // 🔹 Color normal
            imagenBotonCargar.color = Color.white;
        }
    }

    // BOTÓN PLAY
    public void PlayGame()
    {
        if (SaveSystem.ExisteGuardado())
        {
            // ⚠️ Hay partida → mostrar aviso
            panelConfirmacionNuevaPartida.SetActive(true);
        }
        else
        {
            // 🟢 No hay partida → empezar directo
            EmpezarNuevaPartida();
        }
    }

    // BOTÓN CONFIRMAR
    public void ConfirmarNuevaPartida()
    {
        SaveSystem.DeleteSave();
        EmpezarNuevaPartida();
    }

    // BOTÓN CANCELAR
    public void CancelarNuevaPartida()
    {
        panelConfirmacionNuevaPartida.SetActive(false);
    }

    // LÓGICA ORIGINAL
    void EmpezarNuevaPartida()
    {
        GameManager.ResetGame();

        if (RewardManager.Instance != null)
            RewardManager.Instance.ResetUI();

        SceneManager.LoadScene("Plaza");
    }
}
