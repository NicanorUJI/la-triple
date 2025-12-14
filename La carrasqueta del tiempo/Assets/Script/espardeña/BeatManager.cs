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

    void Start()  //                       INICIO

    {
        wait1s = new WaitForSeconds(1f);
        waitHalf = new WaitForSeconds(0.5f);
        waitEndDelay = new WaitForSeconds(3f);

        if (countdownImage != null) countdownImage.gameObject.SetActive(false);
        if (startScreen != null) startScreen.SetActive(true);
        if (returnButton != null) returnButton.SetActive(false);
        if (endScreen != null) endScreen.SetActive(false);
    }

    public void OnStartButtonPressed()  //                          BOTON COMENZAR
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


    //               CUENTA ATRÁS Y COMIENZO MÚSICA

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


    void Update()       //                  BUCLE
    {
        if (!gameStarted) return;
        if (musicSource == null || !musicSource.isPlaying) return;

        if (musicSource.time >= nextBeatTime)
        {
            RegisterBeat(musicSource.time, 1f);
            nextBeatTime += beatInterval;
        }

        if (Input.GetKeyDown(KeyCode.Space)) //              SE GOLPEA LA NOTA CON SPACE
        {
            bool hitAnyNote = false;

            GameObject[] notes = GameObject.FindGameObjectsWithTag("Beat");
            foreach (GameObject note in notes)
            {
                Note noteScript = note.GetComponent<Note>();
                if (noteScript != null && noteScript.CanBePressed())
                {
                    hitAnyNote = true;

                    if (RhythmGameManager.instance != null)
                        RhythmGameManager.instance.NoteHit();

                    Destroy(note);

                    break;
                }
            }

            if (!hitAnyNote)
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

        ShowEndScreen();
    }

    void ShowEndScreen()  //                                      PANTALLA FINAL
    {
        if (endScreen != null) endScreen.SetActive(true);
        if (returnButton != null) returnButton.SetActive(false);

        if (finalScoreText != null && RhythmGameManager.instance != null)
        {
            int finalScore = RhythmGameManager.instance.CurrentScore;
            int hits = RhythmGameManager.instance.HitNotes;
            int total = RhythmGameManager.instance.TotalNotes;

            float hitRate = total > 0 ? (float)hits / total : 0f;

            if (extraFinalText != null)
            {
                string mensajeMotivacional =
                    hitRate >= 0.50f ? "¡Molt bé, has guanyat!" :
                    hitRate >= 0.40f ? "Hmm, sabem que pots millorar." :

                finalScoreText.text = $"Has acertat: {finalScore} de {total} notes";

                extraFinalText.text = $"{mensajeMotivacional}";
            }
        }
    }

    void SpawnBeatVisual(float beatEnergy)   //               SPAWNEAR NOTAS
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

        if (RhythmGameManager.instance != null)
            RhythmGameManager.instance.RegisterNote();
    }
    void ClearBeats()
    {
        GameObject[] beats = GameObject.FindGameObjectsWithTag("Beat");
        for (int i = 0; i < beats.Length; i++)
            Destroy(beats[i]);
    }

    public void OnSalirButtonPressed()
    {
        if (!string.IsNullOrEmpty(escenaSalir))
            SceneManager.LoadScene(escenaSalir);
    }
}