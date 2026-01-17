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
    // Arrastra aquí los objetos de la Jerarquía
    public SpriteRenderer characterRenderer; // Tu personaje
    public SpriteRenderer bocadilloRenderer; // El NUEVO objeto "Visual_Bocadillo"

    [Header("=== LAS 'FOTOS' (Sprites) ===")]
    
    [Header("Estado: IDLE (Normal)")]
    public Sprite charIdle; 
    public Sprite textIdle; // DÉJALO VACÍO (None) si no quieres texto al esperar

    [Header("Estado: HIT (Acierto)")]
    public Sprite charHit;  // Cara feliz
    public Sprite textHit;  // Imagen "Bé"

    [Header("Estado: FAIL (Fallo)")]
    public Sprite charFail; // Cara triste
    public Sprite textFail; // Imagen "Mal"

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
        
        // Disparamos la pareja de ACIERTO
        TriggerFeedback(charHit, textHit);
    }

    public void MissNote()
    {
        currentScore--;
        if (currentScore < 0) currentScore = 0;
        UpdateScoreText();
        
        // Disparamos la pareja de FALLO
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

    // FUNCIÓN CLAVE: Cambia las dos imágenes a la vez
    private void SetVisuals(Sprite face, Sprite text)
    {
        // 1. Cambiar la cara
        if (characterRenderer != null)
        {
            characterRenderer.sprite = face;
        }

        // 2. Cambiar el texto
        if (bocadilloRenderer != null)
        {
            if (text == null)
            {
                // TRUCO: Si no hay imagen de texto (Idle), apagamos el renderer
                // para que no se vea un cuadrado blanco.
                bocadilloRenderer.enabled = false;
            }
            else
            {
                // Si hay imagen, lo encendemos y la ponemos.
                Debug.Log("INTENTANDO MOSTRAR: " + text.name); // <--- MIRA LA CONSOLA
                bocadilloRenderer.enabled = true;
                bocadilloRenderer.sprite = text;
            }
        }
    }

    // Inicia el cambio temporal
    private void TriggerFeedback(Sprite FaceSprite, Sprite TextSprite)
    {
        StopCoroutine(nameof(ResetToIdle));
        SetVisuals(FaceSprite, TextSprite);
        StartCoroutine(nameof(ResetToIdle));
    }


    public void MostrarPantallaFinal()
    {
        // Primero calculamos la victoria
        haGanado = CalcularVictoria();

        // Activamos el panel final
        if (panelFin != null)
            panelFin.SetActive(true);

        // Texto y sprite según victoria o derrota
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
        // Ejemplo: consideramos victoria si se acierta al menos el 50% de las notas
        if (totalNotes == 0) return false; // seguridad
        float ratio = (float)currentScore / totalNotes;
        return ratio >= 0.5f;
    }
    public void ResetGame()
    {
        // Recarga la escena actual para reiniciar el minijuego
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    



    private IEnumerator ResetToIdle()
    {
        yield return new WaitForSeconds(feedbackDuration);
        // Volver al estado normal
        SetVisuals(charIdle, textIdle);
    }
}