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
    public Sprite textIdle; 

    [Header("Estado: HIT (Acierto)")]
    public Sprite charHit;  
    public Sprite textHit;  

    [Header("Estado: FAIL (Fallo)")]
    public Sprite charFail; 
    public Sprite textFail; 

    // --- BLOQUE DE AUDIO ---
    [Header("=== AUDIO ===")]
    public AudioSource audioSource; // Arrastra aquí el componente AudioSource
    public AudioClip hitSound;      // Sonido de acierto (nota)
    public AudioClip failSound;     // Sonido de error (nota)
    [Space(10)]
    public AudioClip winSound;      // NUEVO: Sonido al ganar la partida
    public AudioClip loseSound;     // NUEVO: Sonido al perder la partida
    public AudioClip click;      // NUEVO: Sonido al ganar la partida

    // -----------------------------

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

        // Reproducir sonido de acierto
        PlaySound(hitSound);
    }

    public void MissNote()
    {
        currentScore--;
        if (currentScore < 0) currentScore = 0;
        UpdateScoreText();
        TriggerFeedback(charFail, textFail);

        // Reproducir sonido de fallo
        PlaySound(failSound);
    }

    public void NoteLost()
    {
        TriggerFeedback(charFail, textFail);

        // Reproducir sonido de fallo al perder nota
        PlaySound(failSound);
    }

    // --- GESTIÓN DE SONIDO ---
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            // Usamos PlayOneShot para que los sonidos puedan solaparse 
            audioSource.PlayOneShot(clip);
        }
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
            bocadilloRenderer.enabled = true; 
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
        SetVisuals(charIdle, textIdle);
    }

    // --- PANTALLA FINAL ---
    public void MostrarPantallaFinal()
    {
        haGanado = CalcularVictoria();
        if (panelFin != null) panelFin.SetActive(true);

        if (haGanado)
        {
            if (botonContinuar != null) botonContinuar.interactable = true;
            if (textoResultado != null) textoResultado.text = $"Molt bé! Has aconseguit {currentScore}/{totalNotes} notes.";
            if (imagenResultado != null) imagenResultado.sprite = spriteVictoria;

            // --- NUEVO: Sonido de Victoria ---
            PlaySound(winSound);
        }
        else
        {
            if (botonContinuar != null) botonContinuar.interactable = false;
            if (textoResultado != null) textoResultado.text = $"Ho sentim, només has aconseguit {currentScore}/{totalNotes} notes.";
            if (imagenResultado != null) imagenResultado.sprite = spriteDerrota;

            // --- NUEVO: Sonido de Derrota ---
            PlaySound(loseSound);
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
        StartCoroutine(ResetGameSequence());
    }

    // Esta es la secuencia con espera
    private IEnumerator ResetGameSequence()
    {
        // 1. Si hay sonido y audio source, lo reproducimos y esperamos
        if (click != null && audioSource != null)
        {
            audioSource.PlayOneShot(click);
            
            // Esperamos exactamente lo que dura el clip de audio
            yield return new WaitForSeconds(click.length);
        }

        // 2. Una vez acabado el sonido (o si no había), recargamos la escena
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}