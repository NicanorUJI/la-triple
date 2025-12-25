using UnityEngine;
using TMPro;
using System.Collections;

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

    private IEnumerator ResetToIdle()
    {
        yield return new WaitForSeconds(feedbackDuration);
        // Volver al estado normal
        SetVisuals(charIdle, textIdle);
    }
}