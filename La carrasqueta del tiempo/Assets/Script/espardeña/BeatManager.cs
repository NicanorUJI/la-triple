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
    private float spawnY = 8f, minX = -5f, maxX = 5f, spawnZ = 0f;

    [Header("")]
    public float beatInterval = 0.5f;

    [Header("")]
    public float velocidadCaida = 3f;

    [Header("")]
    public AudioSource musicSource;

    [Header("")]
    public GameObject notaPrefab;

    [Header("")]
    public string escenaSalir;

    [Header(" ")]
    public Image countdownImage;

    [Header("")]
    public Sprite[] countdownSprites;

    [Header("")]
    public float countdownDuration = 3f;

    [Header("")]
    public AudioSource countdownAudioSource;

    [Header("")]
    public AudioClip countdownBeep;

    [Header("")]
    public GameObject startScreen;

    [Header("")]
    public GameObject returnButton;

    [Header("")]
    public GameObject endScreen;

    [Header("")]
    public TMP_Text finalScoreText;

    [Header("")]
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
    waitEndDelay = new WaitForSeconds(3f); // si quieres 3s siempre

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

                yield return wait1s;   // AHORA CACHEADO
            }

            if (countdownSprites.Length > countdownDuration)
            {
                countdownImage.sprite = countdownSprites[(int)countdownDuration];

                if (countdownAudioSource != null && countdownBeep != null)
                    countdownAudioSource.PlayOneShot(countdownBeep);

                yield return waitHalf; // CACHEADO
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
    //                    UPDATE (SPAWN FIJO)
    // ---------------------------------------------------------
    void Update()
    {
        if (!gameStarted) return;
        if (musicSource == null || !musicSource.isPlaying) return;

        if (musicSource.time >= nextBeatTime)
        {
            RegisterBeat(musicSource.time, 1f);
            nextBeatTime += beatInterval;
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

        yield return waitEndDelay;  // CACHEADO

        ShowEndScreen();
    }

    void ShowEndScreen()
    {
        if (endScreen != null) endScreen.SetActive(true);
        if (returnButton != null) returnButton.SetActive(false);

        if (finalScoreText != null && RhythmGameManager.instance != null)
        {
            int hits = RhythmGameManager.instance.HitNotes;
            int total = RhythmGameManager.instance.TotalNotes;

            finalScoreText.text = $"Has acertat {hits} de {total} notes";

            float hitRate = (float)hits / Mathf.Max(1, total);

            if (extraFinalText != null)
            {
                extraFinalText.text =
                    hitRate >= 0.95f ? "¡Increíble! Eres un maestre del ritme." :
                    hitRate >= 0.80f ? "¡Molt bé!" :
                    hitRate >= 0.60f ? "Nada mal, pero pots millorar." :
                    "Continua practicant...";
            }
        }
    }
    // ---------------------------------------------------------
    //                   SPAWN DE NOTAS
    // ---------------------------------------------------------
    void SpawnBeatVisual(float beatEnergy)
    {
        if (notaPrefab == null) return;

        Vector3 pos = new(Random.Range(minX, maxX), spawnY, spawnZ);
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
        if (!string.IsNullOrEmpty(escenaSalir))
            SceneManager.LoadScene(escenaSalir);
    }
}
