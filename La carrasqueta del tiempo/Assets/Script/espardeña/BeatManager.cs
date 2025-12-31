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

    [Header("Configuración Velocidad")]
    public float velocidadCaida = 0.5f; 
    
    [Tooltip("Cuánto aumenta la velocidad por segundo. Pon 0 para velocidad constante.")]
    // Variable placeholder por si quieres implementar aceleración futura

    [Header("Audio")]
    public AudioSource musicSource;

    [Header("Prefabs y Referencias")]
    public GameObject[] notasPrefabs; // Asegúrate de que los prefabs tengan los tags asignados en el Inspector
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

    // Definimos los tags aquí para evitar errores de escritura
    private string tag1 = "nota1";
    private string tag2 = "nota2";
    private string tag3 = "nota3";

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
        if (musicSource != null) musicSource.Stop();
        ClearBeats();
        
        if (startScreen != null) startScreen.SetActive(true);
        if (returnButton != null) returnButton.SetActive(false);
        if (endScreen != null) endScreen.SetActive(false);

        if (RhythmGameManager.instance != null)
            RhythmGameManager.instance.ResetScore();

        gameStarted = false;
        topBeats.Clear();
    }

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
            countdownImage.gameObject.SetActive(false);
        }

        nextBeatTime = 0f;
        gameStarted = true;

        if (musicSource != null && musicSource.clip != null)
        {
            musicSource.PlayScheduled(AudioSettings.dspTime + 0.1);
            StartCoroutine(ShowEndScreenWithDelay());
        }
    }

    void Update()
    {
        if (!gameStarted) return;

        // --- PARTE 1: GENERACIÓN DE NOTAS ---
        if (musicSource != null && musicSource.isPlaying)
        {
            if (musicSource.time >= nextBeatTime)
            {
                RegisterBeat(musicSource.time, 1f);
                nextBeatTime += beatInterval;
            }
        }

        // --- PARTE 2: INPUT DEL JUGADOR (MULTITECLA) ---
        
        // Verificamos Tecla A para nota1
        VerificarInput(KeyCode.A, tag1);

        // Verificamos Tecla S para nota2
        VerificarInput(KeyCode.S, tag2);

        // Verificamos Tecla D para nota3
        VerificarInput(KeyCode.D, tag3);
    }

    // --- NUEVA FUNCIÓN PARA GESTIONAR CADA TECLA ---
    void VerificarInput(KeyCode tecla, string tagObjetivo)
    {
        if (Input.GetKeyDown(tecla))
        {
            // Solo buscamos objetos con el Tag específico de esa tecla
            GameObject[] notes = GameObject.FindGameObjectsWithTag(tagObjetivo);
            
            Note bestNote = null;
            float minY = float.MaxValue;

            foreach (GameObject noteObj in notes)
            {
                Note noteScript = noteObj.GetComponent<Note>();
                if (noteScript != null && noteScript.CanBePressed())
                {
                    if (noteObj.transform.position.y < minY)
                    {
                        minY = noteObj.transform.position.y;
                        bestNote = noteScript;
                    }
                }
            }

            if (bestNote != null)
            {
                // ACIERTO
                if (RhythmGameManager.instance != null)
                    RhythmGameManager.instance.NoteHit();
                
                Destroy(bestNote.gameObject);
            }
            else
            {
                // FALLO (Pulsó la tecla pero no había nota de ese tipo cerca)
                if (RhythmGameManager.instance != null)
                    RhythmGameManager.instance.MissNote();
            }
        }
    }

    void StopGame()
    {
        gameStarted = false;
    }

    private IEnumerator ShowEndScreenWithDelay()
    {
        if (musicSource != null && musicSource.clip != null)
            yield return new WaitForSeconds(musicSource.clip.length - 0.1f);

        yield return waitEndDelay;

        StopGame();
        
        if (RhythmGameManager.instance != null)
            RhythmGameManager.instance.MostrarPantallaFinal();
    }

    void SpawnBeatVisual(float beatEnergy)
    {
        if (notasPrefabs == null || notasPrefabs.Length == 0) return;

        int randomIndex = Random.Range(0, notasPrefabs.Length);
        GameObject prefabSeleccionado = notasPrefabs[randomIndex];

        Vector3 pos = new Vector3(Random.Range(minX, maxX), spawnY, 0);
        
        // Al instanciar, el objeto mantendrá el Tag que tenga puesto el Prefab
        GameObject note = Instantiate(prefabSeleccionado, pos, Quaternion.identity);

        Note noteScript = note.GetComponent<Note>();
        if (noteScript != null)
            noteScript.fallSpeed = velocidadCaida; 
    }

    void RegisterBeat(float time, float energy)
    {
        if (topBeats.Count > 50) topBeats.RemoveAt(0);
        
        topBeats.Add(new BeatInfo { time = time, energy = energy });
        SpawnBeatVisual(energy);

        if (RhythmGameManager.instance != null)
            RhythmGameManager.instance.RegisterNote();
    }

    // --- ACTUALIZADO PARA LIMPIAR TODOS LOS TAGS ---
    void ClearBeats()
    {
        // Creamos una lista temporal con todos los tags a limpiar
        string[] tagsToClean = { tag1, tag2, tag3 };

        foreach (string t in tagsToClean)
        {
            GameObject[] beats = GameObject.FindGameObjectsWithTag(t);
            foreach(var beat in beats) Destroy(beat);
        }
    }

    public void OnSalirButtonPressed()
    {
        if (!string.IsNullOrEmpty(escenaSalir))
            SceneManager.LoadScene(escenaSalir);
    }
}