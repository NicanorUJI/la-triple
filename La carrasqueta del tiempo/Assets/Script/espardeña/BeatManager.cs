using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class AutoBeatDetectorTop50 : MonoBehaviour
{
    private List<BeatInfo> topBeats = new List<BeatInfo>();
    private bool gameStarted = false;
    private WaitForSeconds wait1s, waitHalf, waitEndDelay;
    private float nextBeatTime = 0f;
    private float spawnY = 0f, minX = -1f, maxX = -0.3f;

    [Header("Configuración Rítmica")]
    public float beatInterval = 0.5f;

    [Header("Configuración Notas")]
    public float velocidadCaida = 1f;

    [Header("Audio")]
    public AudioSource musicSource;

    [Header("Prefabs y Referencias")]
    public GameObject notaPrefab;
    public string escenaSalir;

    [Header("Cuenta Atrás")]
    public Image countdownImage;
    public Sprite[] countdownSprites;
    public float countdownDuration = 3f;
    public AudioSource countdownAudioSource;
    public AudioClip countdownBeep;

    [Header("UI Pantallas")]
    public GameObject startScreen;
    public GameObject returnButton;
    public GameObject endScreen;
    public TMP_Text finalScoreText;
    public TMP_Text extraFinalText;


    [System.Serializable]
    public class BeatInfo { public float time, energy; }

    // ---------------------------------------------------------
    //                       INICIO
    // ---------------------------------------------------------
    void Start()
    {
        wait1s = new WaitForSeconds(1f);
        waitHalf = new WaitForSeconds(0.5f);
        waitEndDelay = new WaitForSeconds(3f);

        if (countdownImage != null) countdownImage.gameObject.SetActive(false);
        if (startScreen != null) startScreen.SetActive(true);
        if (returnButton != null) returnButton.SetActive(false);
        if (endScreen != null) endScreen.SetActive(false);
    }

    public void OnStartButtonPressed()
    {
        if (startScreen != null) startScreen.SetActive(false);
        if (returnButton != null) returnButton.SetActive(true);

        StartCoroutine(StartMusicWithCountdown());
    }

    public void OnReturnButtonPressed()
    {
        StopAllCoroutines();

        if (musicSource != null)
            musicSource.Stop();

        ClearBeats();

        if (startScreen != null) startScreen.SetActive(true);
        if (returnButton != null) returnButton.SetActive(false);
        if (endScreen != null) endScreen.SetActive(false);

        if (RhythmGameManager.instance != null)
            RhythmGameManager.instance.ResetScore();

        gameStarted = false;
        topBeats.Clear();
    }

    // ---------------------------------------------------------
    //               CUENTA ATRÁS Y COMIENZO MÚSICA
    // ---------------------------------------------------------
    private IEnumerator StartMusicWithCountdown()
    {
        if (countdownSprites != null && countdownSprites.Length > 0 && countdownImage != null)
        {
            countdownImage.gameObject.SetActive(true);

            for (int i = 0; i < countdownDuration; i++)
            {
                countdownImage.sprite = countdownSprites[Mathf.Clamp(i, 0, countdownSprites.Length - 1)];

                if (countdownAudioSource != null && countdownBeep != null)
                    countdownAudioSource.PlayOneShot(countdownBeep);

                yield return wait1s;
            }

            if (countdownSprites.Length > countdownDuration)
            {
                countdownImage.sprite = countdownSprites[(int)countdownDuration];
                if (countdownAudioSource != null && countdownBeep != null)
                    countdownAudioSource.PlayOneShot(countdownBeep);
                yield return waitHalf;
            }

            countdownImage.gameObject.SetActive(false);
        }

        nextBeatTime = 0f;
        gameStarted = true;

        if (musicSource != null && musicSource.clip != null)
        {
            musicSource.PlayScheduled(AudioSettings.dspTime + 0.1);
            Invoke(nameof(StopGame), musicSource.clip.length - 0.1f);
            StartCoroutine(ShowEndScreenWithDelay());
        }
    }

    // ---------------------------------------------------------
    //                    UPDATE (LOGICA ARREGLADA)
    // ---------------------------------------------------------
    void Update()
    {
        if (!gameStarted) return;
        if (musicSource == null || !musicSource.isPlaying) return;

        // Generar Beats
        if (musicSource.time >= nextBeatTime)
        {
            RegisterBeat(musicSource.time, 1f);
            nextBeatTime += beatInterval;
        }

        // Input del jugador
        if (Input.GetKeyDown(KeyCode.Space))
        {
            bool hitAnyNote = false;

            GameObject[] notes = GameObject.FindGameObjectsWithTag("Beat");
            foreach (GameObject note in notes)
            {
                Note noteScript = note.GetComponent<Note>();
                // Comprobamos si la nota es válida para ser pulsada
                if (noteScript != null && noteScript.CanBePressed())
                {
                    hitAnyNote = true;

                    // --- CORRECCIÓN AQUÍ ---
                    // 1. Avisamos al Manager que acertamos
                    if (RhythmGameManager.instance != null)
                        RhythmGameManager.instance.NoteHit();
                    
                    // 2. Destruimos la nota para que no falle al salir de pantalla ni se pulse dos veces
                    Destroy(note); 
                    // -----------------------

                    break; // Solo permitimos acertar una nota por pulsación
                }
            }

            // Si pulsamos espacio pero no había nota
            if (!hitAnyNote)
            {
                if (RhythmGameManager.instance != null)
                    RhythmGameManager.instance.MissNote();
            }
        }
    }

    // ---------------------------------------------------------
    //               FIN DE CANCIÓN Y PANTALLA FINAL
    // ---------------------------------------------------------
    void StopGame()
    {
        gameStarted = false;
    }

    private IEnumerator ShowEndScreenWithDelay()
    {
        if (musicSource != null && musicSource.clip != null)
            yield return new WaitForSeconds(musicSource.clip.length - 0.1f);

        yield return waitEndDelay;

    
        RhythmGameManager.instance.MostrarPantallaFinal();
    }

    /*private int calcularVictoria()
    {
        if (endScreen != null) endScreen.SetActive(true);
        if (returnButton != null) returnButton.SetActive(false);

        if (finalScoreText != null && RhythmGameManager.instance != null)
        {
            // 1. Obtenemos los datos
            int finalScore = RhythmGameManager.instance.CurrentScore; // Puntuación con restas
            int hits = RhythmGameManager.instance.HitNotes;           // Aciertos puros
            int total = RhythmGameManager.instance.TotalNotes;        // Total de notas

            // 2. CAMBIO AQUÍ: Mostramos la PUNTUACIÓN (Score) en el texto grande

            // 3. Calculamos la precisión para el mensaje extra
            float hitRate = total > 0 ? (float)hits / total : 0f;

            if (extraFinalText != null)
            {
                // Puedes añadir el detalle de aciertos aquí si quieres
                string mensajeMotivacional =
                    hitRate >= 0.50f ? "¡Molt bé!" :
                    hitRate >= 0.40f ? "Nada mal, pero pots millorar." :
                    "Continua practicant...";

                // Muestra: "¡Increíble! (40/50 notas)"
                finalScoreText.text = $"Puntuació Final: {finalScore} de {total} notes";

                extraFinalText.text = $"{mensajeMotivacional}";
            }
        }
    }*/

    // ---------------------------------------------------------
    //                   SPAWN DE NOTAS
    // ---------------------------------------------------------
    void SpawnBeatVisual(float beatEnergy)
    {
        if (notaPrefab == null) return;

        Vector3 pos = new Vector3(Random.Range(minX, maxX), spawnY, 0);
        GameObject note = Instantiate(notaPrefab, pos, Quaternion.identity);

        float fallSpeed = velocidadCaida;

        Note noteScript = note.GetComponent<Note>();
        if (noteScript != null)
            noteScript.fallSpeed = fallSpeed;
    }

    void RegisterBeat(float time, float energy)
    {
        topBeats.Add(new BeatInfo { time = time, energy = energy });
        topBeats.Sort((a, b) => b.energy.CompareTo(a.energy));

        if (topBeats.Count > 50)
            topBeats.RemoveAt(50);

        SpawnBeatVisual(energy);

        // Esto cuenta el TOTAL de notas generadas
        if (RhythmGameManager.instance != null)
            RhythmGameManager.instance.RegisterNote();
    }

    // ---------------------------------------------------------
    //                     UTILIDADES
    // ---------------------------------------------------------
    void ClearBeats()
    {
        GameObject[] beats = GameObject.FindGameObjectsWithTag("Beat");
        for (int i = 0; i < beats.Length; i++)
            Destroy(beats[i]);
    }

    public void OnSalirButtonPressed()
    {
        GameManager.Change("Act2_Q_ESP_HasEspart");

        if (!string.IsNullOrEmpty(escenaSalir))
            SceneManager.LoadScene(escenaSalir);
    }
}