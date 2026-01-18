using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RhythmGameManager : MonoBehaviour
{
    public static RhythmGameManager instance;

    [Header("UI Puntuación")]
    public TMP_Text scoreText;

    [Header("=== LOS 'MARCOS' DE LA ESCENA ===")]
    public SpriteRenderer characterRenderer; // Tu personaje
    public SpriteRenderer bocadilloRenderer; // El objeto "Visual_Bocadillo"

    [Header("=== LAS 'FOTOS' (Sprites) ===")]
    
    [Header("Estado: IDLE (Normal)")]
    public Sprite charIdle; 
    // IMPORTANTE: Ahora necesitas poner una imagen aquí (ej: burbuja vacía o "...")
    // para que no desaparezca el bocadillo.
    public Sprite textIdle; 

    [Header("Estado: HIT (Acierto)")]
    public Sprite charHit;  
    public Sprite textHit;  

    [Header("Estado: FAIL (Fallo)")]
    public Sprite charFail; 
    public Sprite textFail; 

    [Header("Configuración")]
    public float feedbackDuration = 0.5f;

    [Header("Panel Fin de Juego")]
    public GameObject panelFin;
    public Image imagenResultado;
    public TMP_Text textoResultado;
    public Button botonContinuar;
    public Button botonReintentar;
    public Sprite spriteVictoria;
    public Sprite spriteDerrota;

    private bool haGanado = false;

    // Variables internas
    private int currentScore = 0;
    private int hitNotes = 0;
    private int totalNotes = 0;

    public int HitNotes => hitNotes;
    public int TotalNotes => totalNotes;
    public int CurrentScore => currentScore;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UpdateScoreText();
        
        // Aseguramos que el renderer del bocadillo empiece activado
        if(bocadilloRenderer != null) bocadilloRenderer.enabled = true;

        // Estado inicial: Idle
        SetVisuals(charIdle, textIdle);
    }

    // --- LOGICA DE JUEGO ---

    public void RegisterNote() { totalNotes++; }

    public void NoteHit()
    {
        hitNotes++;
        currentScore++;
        UpdateScoreText();
        TriggerFeedback(charHit, textHit);
    }

    public void MissNote()
    {
        currentScore--;
        if (currentScore < 0) currentScore = 0;
        UpdateScoreText();
        TriggerFeedback(charFail, textFail);
    }

    public void NoteLost()
    {
        TriggerFeedback(charFail, textFail);
    }

    // --- LOGICA VISUAL ---

    void UpdateScoreText()
    {
        if (scoreText != null) scoreText.text = "Punts: " + currentScore;
    }

    public void ResetScore()
    {
        currentScore = 0;
        hitNotes = 0;
        totalNotes = 0;
        UpdateScoreText();
        SetVisuals(charIdle, textIdle);
    }

    // --- AQUÍ ESTÁ EL CAMBIO PRINCIPAL ---
    private void SetVisuals(Sprite face, Sprite text)
    {
        // 1. Cambiar la cara
        if (characterRenderer != null)
        {
            characterRenderer.sprite = face;
        }

        // 2. Cambiar el texto (Sin apagar el objeto)
        if (bocadilloRenderer != null)
        {
            // Nos aseguramos de que siempre esté visible
            bocadilloRenderer.enabled = true; 

            // Simplemente cambiamos la foto. 
            // El objeto "Visual_Bocadillo" se queda quieto, solo cambia su pintura.
            bocadilloRenderer.sprite = text;
        }
    }

    private void TriggerFeedback(Sprite FaceSprite, Sprite TextSprite)
    {
        StopCoroutine(nameof(ResetToIdle));
        SetVisuals(FaceSprite, TextSprite);
        StartCoroutine(nameof(ResetToIdle));
    }

    private IEnumerator ResetToIdle()
    {
        yield return new WaitForSeconds(feedbackDuration);
        // Volver al estado normal (la burbuja cambiará a la imagen "textIdle")
        SetVisuals(charIdle, textIdle);
    }

    // ... (El resto de funciones de Fin de Juego siguen igual) ...
    public void MostrarPantallaFinal()
    {
        haGanado = CalcularVictoria();
        if (panelFin != null) panelFin.SetActive(true);

        if (haGanado)
        {
            if (botonContinuar != null) botonContinuar.interactable = true;
            if (textoResultado != null) textoResultado.text = $"Molt bé! Has aconseguit {currentScore}/{totalNotes} notes.";
            if (imagenResultado != null) imagenResultado.sprite = spriteVictoria;
        }
        else
        {
            if (botonContinuar != null) botonContinuar.interactable = false;
            if (textoResultado != null) textoResultado.text = $"Ho sentim, només has aconseguit {currentScore}/{totalNotes} notes.";
            if (imagenResultado != null) imagenResultado.sprite = spriteDerrota;
        }
    }

    private bool CalcularVictoria()
    {
        if (totalNotes == 0) return false;
        float ratio = (float)currentScore / totalNotes;
        return ratio >= 0.5f;
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}