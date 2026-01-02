using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class FinalController : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text finalText;
    public RectTransform textRect;
    public Button nextButton; // Botón para pasar de Parte 1 a Parte 2
    public Button menuButton; // Botón para volver al menú

    [Header("Scene names")]
    public string menuSceneName = "Escena Menú";

    [Header("Textos")]
    [TextArea(5, 10)] public string textPart1;
    [TextArea(5, 10)] public string textPart2;

    [Header("Efectos")]
    public float charDelay = 0.04f;
    public float scrollSpeed = 15f;

    private string currentText;
    private int charIndex;
    private float charTimer;

    private enum Estado { Parte1, EsperandoBoton, Parte2, Terminado }
    private Estado estadoActual = Estado.Parte1;

    private void Start()
    {
        // Inicializa la primera parte
        IniciarParte(textPart1);

        // Configura botones
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(false);
            nextButton.onClick.AddListener(PasarAParte2);
        }

        if (menuButton != null)
        {
            menuButton.gameObject.SetActive(false);
            menuButton.onClick.AddListener(GoToMenu);
        }
    }

    private void Update()
    {
        EscribirTexto();
        SubirTexto();
        ControlEstados();
    }

    private void IniciarParte(string texto)
    {
        currentText = texto;
        finalText.text = "";
        charIndex = 0;
        charTimer = 0f;
        textRect.anchoredPosition = Vector2.zero;
    }

    private void EscribirTexto()
    {
        if (charIndex >= currentText.Length) return;

        charTimer += Time.deltaTime;
        if (charTimer >= charDelay)
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

    private void ControlEstados()
    {
        switch (estadoActual)
        {
            case Estado.Parte1:
                if (charIndex >= currentText.Length)
                {
                    // Cuando termina la Parte 1, muestra el botón de siguiente
                    estadoActual = Estado.EsperandoBoton;
                    if (nextButton != null)
                        nextButton.gameObject.SetActive(true);
                }
                break;

            case Estado.Parte2:
                if (charIndex >= currentText.Length)
                {
                    // Cuando termina la Parte 2, muestra el botón de menú
                    estadoActual = Estado.Terminado;
                    if (menuButton != null)
                        menuButton.gameObject.SetActive(true);
                }
                break;
        }
    }

    private void PasarAParte2()
    {
        if (nextButton != null)
            nextButton.gameObject.SetActive(false);

        estadoActual = Estado.Parte2;
        IniciarParte(textPart2);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}
