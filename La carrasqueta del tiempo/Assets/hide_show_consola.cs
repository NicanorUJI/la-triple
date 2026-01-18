using UnityEngine;
using UnityEngine.SceneManagement;

public class Hide_Show_PauseButton_OnTrigger : MonoBehaviour
{
    public bool hide = false;

    public string[] requiredFlags;
    public string[] forbiddenFlags;

    private GameObject pauseButton;
    private int uiLayer;

    void Awake()
    {
        uiLayer = LayerMask.NameToLayer("UI");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        FindPauseButton();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindPauseButton();
    }

    private void FindPauseButton()
    {
        if (pauseButton != null)
            return;

        GameObject[] allObjects = FindObjectsOfType<GameObject>(true);

        foreach (GameObject obj in allObjects)
        {
            if (obj.name == "BotonPausa" && obj.layer == uiLayer)
            {
                pauseButton = obj;
                break;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (!CheckFlags())
            return;

        if (pauseButton == null)
            return;

        pauseButton.SetActive(!hide);

        if (!MenuPausa.Instance.botonDesbloqueado) // solo la primera vez
        {
            MenuPausa.Instance.botonDesbloqueado = true;
            Debug.Log("✅ Botón de pausa desbloqueado por trigger");
        }
    }

    private bool CheckFlags()
    {
        if (requiredFlags != null)
        {
            foreach (var flag in requiredFlags)
            {
                if (!string.IsNullOrEmpty(flag) && !GameManager.Check(flag))
                    return false;
            }
        }

        if (forbiddenFlags != null)
        {
            foreach (var flag in forbiddenFlags)
            {
                if (!string.IsNullOrEmpty(flag) && GameManager.Check(flag))
                    return false;
            }
        }

        return true;
    }
}
