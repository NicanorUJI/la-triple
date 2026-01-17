using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

#region PAGE DATA
[System.Serializable]
public class BookPage
{
    [TextArea(5, 10)]
    public string text;

    public Sprite image;      // Imagen opcional
    public bool centerText;   // Texto centrado o no
}
#endregion

public class BookMultiPageController : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text bookText;
    public RectTransform textRect;
    public Image pageImage;
    public Button returnButton;

    [Header("Páginas del libro")]
    public BookPage[] pages;

    [Header("Animación")]
    public Animator bookAnimator;
    public float animDuration = 2f;

    [Header("Escritura")]
    public float charsPerSecond = 25f;
    public float scrollSpeed = 15f;

    [Header("Texto - Posiciones")]
    public Vector2 normalTextPosition;
    public Vector2 centeredTextPosition;

    [Header("Escena menú")]
    public string menuSceneName = "Escena Menú";

    [Header("Avance automático")]
    public float waitAfterPageEnd = 2f; // Tiempo en segundos antes de pasar a la siguiente página

    // ───────────── PRIVADOS ─────────────

    private int currentPage = 0;
    private string currentText;
    private int charIndex;
    private float charTimer;
    private bool writing;

    // ───────────── UNITY ─────────────

    private void Start()
    {
        returnButton.gameObject.SetActive(false);
        returnButton.onClick.AddListener(ReturnToMenu);

        StartBook();
    }

    private void Update()
    {
        if (!writing) return;

        WriteText();
        ScrollText();

        if (charIndex >= currentText.Length && writing)
        {
            writing = false;

            if (currentPage < pages.Length - 1)
            {
                // Avanza automáticamente a la siguiente página después de waitAfterPageEnd segundos
                Invoke(nameof(NextPage), waitAfterPageEnd);
            }
            else
            {
                // Última página, mostramos botón de retorno
                returnButton.gameObject.SetActive(true);
            }
        }
    }

    // ───────────── LIBRO ─────────────

    private void StartBook()
    {
        currentPage = 0;
        PlayAnimation();
        Invoke(nameof(StartCurrentPage), animDuration);
    }

    private void StartCurrentPage()
    {
        BookPage page = pages[currentPage];

        // ─ Imagen (opcional)
        if (pageImage != null)
        {
            if (page.image != null)
            {
                pageImage.sprite = page.image;
                pageImage.gameObject.SetActive(true);
            }
            else
            {
                pageImage.gameObject.SetActive(false);
            }
        }

        // ─ Texto centrado o normal
        if (page.centerText)
        {
            bookText.alignment = TextAlignmentOptions.Center;
            textRect.anchoredPosition = centeredTextPosition;
        }
        else
        {
            bookText.alignment = TextAlignmentOptions.TopLeft;
            textRect.anchoredPosition = normalTextPosition;
        }

        StartWriting(page.text);
    }

    private void StartWriting(string text)
    {
        currentText = text;
        bookText.text = "";
        charIndex = 0;
        charTimer = 0f;
        writing = true;
    }

    // ───────────── TEXTO ─────────────

    private void WriteText()
    {
        charTimer += Time.deltaTime;
        float delay = 1f / charsPerSecond;

        if (charTimer >= delay && charIndex < currentText.Length)
        {
            charTimer = 0f;
            charIndex++;
            bookText.text = currentText.Substring(0, charIndex);
        }
    }

    private void ScrollText()
    {
        if (charIndex < currentText.Length)
        {
            textRect.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
        }
    }

    // ───────────── SIGUIENTE PÁGINA ─────────────

    private void NextPage()
    {
        // Cancelar posibles invocaciones anteriores
        CancelInvoke(nameof(StartCurrentPage));

        // Ocultar texto e imagen ANTES de la animación
        bookText.text = "";
        currentText = "";
        if (pageImage != null)
            pageImage.gameObject.SetActive(false);

        currentPage++;

        PlayAnimation();
        Invoke(nameof(StartCurrentPage), animDuration);
    }

    private void ReturnToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }

    // ───────────── ANIMACIÓN ─────────────

    private void PlayAnimation()
    {
        if (bookAnimator != null)
            bookAnimator.SetTrigger("Play");
    }
}
