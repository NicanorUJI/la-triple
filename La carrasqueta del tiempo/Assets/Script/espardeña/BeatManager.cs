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
    
    // MODIFICADO: Eliminamos minX y maxX, dejamos spawnY
    private float spawnY = 0f; // Asegúrate de ajustar esto en el inspector o inicializarlo donde prefieras

    [Header("Configuración Rítmica")]
    public float beatInterval = 0.5f;

    [Header("Configuración Velocidad")]
    public float velocidadCaida = 5f; 

    // --- NUEVO: CONFIGURACIÓN DE CARRILES ---
    [Header("Configuración de Carriles (Posición X)")]
    [Tooltip("Posición X para el prefab que tenga el Tag 'nota1' (Tecla A)")]
    public float xPosNota1 = -2f; 

    [Tooltip("Posición X para el prefab que tenga el Tag 'nota2' (Tecla S)")]
    public float xPosNota2 = 0f;

    [Tooltip("Posición X para el prefab que tenga el Tag 'nota3' (Tecla D)")]
    public float xPosNota3 = 2f;
    // ----------------------------------------

    [Header("Audio")]
    public AudioSource musicSource;

    [Header("Prefabs y Referencias")]
    public GameObject[] notasPrefabs; 
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

    // Definimos los tags
    private string tag1 = "nota1";
    private string tag2 = "nota2";
    private string tag3 = "nota3";

    void Start()
    {
        // Inicializamos spawnY fuera de la pantalla (puedes ajustar este valor)
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

        // --- PARTE 2: INPUT DEL JUGADOR ---
        VerificarInput(KeyCode.A, tag1);
        VerificarInput(KeyCode.S, tag2);
        VerificarInput(KeyCode.D, tag3);
    }

    void VerificarInput(KeyCode tecla, string tagObjetivo)
    {
        if (Input.GetKeyDown(tecla))
        {
            GameObject[] notes = GameObject.FindGameObjectsWithTag(tagObjetivo);
            
            Note bestNote = null;
            float minY = float.MaxValue;

            foreach (GameObject noteObj in notes)
            {
                Note noteScript = noteObj.GetComponent<Note>();
                // Verificamos si la nota está dentro de la zona de hit (CanBePressed)
                // Opcional: También podrías verificar la distancia en X si quisieras ser muy estricto
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
                if (RhythmGameManager.instance != null)
                    RhythmGameManager.instance.NoteHit();
                
                Destroy(bestNote.gameObject);
            }
            else
            {
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

    // --- FUNCIÓN MODIFICADA: SPAWN EN CARRILES ---
    void SpawnBeatVisual(float beatEnergy)
    {
        if (notasPrefabs == null || notasPrefabs.Length == 0) return;

        // 1. Elegimos un prefab al azar (cada prefab debe tener su Tag asignado en el Inspector del proyecto)
        int randomIndex = Random.Range(0, notasPrefabs.Length);
        GameObject prefabSeleccionado = notasPrefabs[randomIndex];

        // 2. Determinamos la posición X basándonos en el Tag del prefab elegido
        float targetX = 0f;

        if (prefabSeleccionado.CompareTag(tag1))
        {
            targetX = xPosNota1;
        }
        else if (prefabSeleccionado.CompareTag(tag2))
        {
            targetX = xPosNota2;
        }
        else if (prefabSeleccionado.CompareTag(tag3))
        {
            targetX = xPosNota3;
        }
        else
        {
            // Fallback por si el prefab no tiene tag correcto
            Debug.LogWarning("El prefab seleccionado no tiene un tag reconocido (nota1, nota2, nota3). Se usará X=0");
            targetX = 0f;
        }

        Vector3 pos = new Vector3(targetX, spawnY, 0);
        
        // 3. Instanciamos la nota en la posición calculada
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

    void ClearBeats()
    {
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