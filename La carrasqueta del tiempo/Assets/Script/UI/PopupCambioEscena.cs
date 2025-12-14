using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangePopupUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text sceneNameText;

    [Header("Tiempos")]
    [SerializeField] private float fadeDuration = 0.25f;
    [SerializeField] private float visibleTime = 2f;

    private void Reset()
    {
        // Esto se ejecuta cuando añades el script desde el inspector
        canvasGroup = GetComponent<CanvasGroup>();
        if (!sceneNameText)
            sceneNameText = GetComponentInChildren<TMP_Text>();
    }

    private void Awake()
    {
        // Empezamos ocultos
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
    }

    private void Start()
    {
        // Cuando la escena está lista y el UI arrancó, mostramos el popup
        StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        if (canvasGroup == null || sceneNameText == null)
            yield break;

        // Obtener nombre de escena (aquí podrías mapear a un nombre "bonito")
        string sceneName = SceneManager.GetActiveScene().name;
        sceneNameText.text = GetDisplayName(sceneName);

        // --- Fade IN ---
        float t = 0f;
        canvasGroup.alpha = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // --- Tiempo visible ---
        yield return new WaitForSecondsRealtime(visibleTime);

        // --- Fade OUT ---
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;

        // Lo apagamos para que no estorbe
        gameObject.SetActive(false);
    }

    // Aquí puedes mapear nombres técnicos a nombres para el jugador
    private string GetDisplayName(string sceneName)
    {
        // Ejemplos:
        // if (sceneName == "Plaza_Pasado") return "Plaza (Pasado)";
        // if (sceneName == "Carrasqueta") return "La Carrasqueta";

        return sceneName; // Por ahora devolvemos el nombre tal cual
    }
}
