using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class FinalController : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text finalText;
    public RectTransform textRect;
    public Button nextButton;
    public Button menuButton;

    [Header("Fondo")]
    public Animator backgroundAnimator;
    public float backgroundAnimDuration = 2f;

    [Header("Scene names")]
    public string menuSceneName = "Escena Menú";

    [Header("Textos")]
    [TextArea(5, 10)] public string textPart1;
    [TextArea(5, 10)] public string textPart2;

    [Header("Velocidad de escritura")]
    [Tooltip("Caracteres por segundo")]
    [Range(1f, 100f)]
    public float charsPerSecond = 25f;

    [Header("Movimiento del texto")]
    public float scrollSpeed = 15f;

    private string currentText = "";
    private int charIndex;
    private float charTimer;

    private enum Estado
    {
        AnimacionInicial,
        Parte1,
        EsperandoBoton,
        AnimacionParte2,
        Parte2,
        Terminado
    }

    private Estado estadoActual = Estado.AnimacionInicial;

    private void Start()
    {
        finalText.text = "";
        currentText = "";

        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(false);
            nextButton.onClick.AddListener(PulsarNext);
        }

        if (menuButton != null)
        {
            menuButton.gameObject.SetActive(false);
            menuButton.onClick.AddListener(GoToMenu);
        }

        // Animación inicial
        ReproducirAnimacionFondo();
        Invoke(nameof(IniciarParte1), backgroundAnimDuration);
    }

    private void Update()
    {
        if (estadoActual == Estado.Parte1 || estadoActual == Estado.Parte2)
        {
            EscribirTexto();
            SubirTexto();
        }

        ControlEstados();
    }

    // ───────────── TEXTOS ─────────────

    private void IniciarParte1()
    {
        estadoActual = Estado.Parte1;
        IniciarTexto(textPart1);
    }

    private void IniciarParte2()
    {
        estadoActual = Estado.Parte2;
        IniciarTexto(textPart2);
    }

    private void IniciarTexto(string texto)
    {
        currentText = texto;
        finalText.text = "";
        charIndex = 0;
        charTimer = 0f;
        textRect.anchoredPosition = Vector2.zero;
    }

    private void EscribirTexto()
    {
        if (string.IsNullOrEmpty(currentText)) return;
        if (charIndex >= currentText.Length) return;

        charTimer += Time.deltaTime;
        float delay = 1f / charsPerSecond;

        if (charTimer >= delay)
        {
            charTimer = 0f;
            charIndex++;
            finalText.text = currentText.Substring(0, charIndex);
        }
    }

    private void SubirTexto()
    {
        if (charIndex < currentText.Length)
        {
            textRect.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
        }
    }

    // ───────────── ESTADOS ─────────────

    private void ControlEstados()
    {
        switch (estadoActual)
        {
            case Estado.Parte1:
                if (charIndex >= currentText.Length)
                {
                    estadoActual = Estado.EsperandoBoton;
                    if (nextButton != null)
                        nextButton.gameObject.SetActive(true);
                }
                break;

            case Estado.Parte2:
                if (charIndex >= currentText.Length)
                {
                    estadoActual = Estado.Terminado;
                    if (menuButton != null)
                        menuButton.gameObject.SetActive(true);
                }
                break;
        }
    }

    // ───────────── BOTONES ─────────────

    private void PulsarNext()
    {
        if (nextButton != null)
            nextButton.gameObject.SetActive(false);

        // Oculta el texto 1 inmediatamente
        finalText.text = "";
        currentText = "";

        estadoActual = Estado.AnimacionParte2;

        ReproducirAnimacionFondo();
        Invoke(nameof(IniciarParte2), backgroundAnimDuration);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }

    // ───────────── FONDO ─────────────

    private void ReproducirAnimacionFondo()
    {
        if (backgroundAnimator != null)
        {
            backgroundAnimator.SetTrigger("Play");
        }
    }
}
