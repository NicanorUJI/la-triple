using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AutoBeatDetectorTop50 : MonoBehaviour
{
    private bool gameStarted = false;

    [Header("Velocidad de caída dinámica")]
    public float initialFallSpeed = 3f;    // velocidad mínima
    public float maxFallSpeed = 20f;        // velocidad máxima
    public float speedIncreaseDuration = 60f; // tiempo hasta alcanzar la velocidad máxima
    public float energySpeedMultiplier = 10f;  // cuánto afecta la energía del beat

    [Header("Audio")]
    public AudioSource musicSource;

    [Header("Visualización de beats")]
    public GameObject beatPrefab;
    public float spawnY = 8f;
    public float minX = -5f;
    public float maxX = 5f;
    public float spawnZ = 0f;

    [Header("Cambio de escena")]
    public string escenaSalir; // nombre de la escena al salir desde el inspector

    [Header("Contador visual de inicio")]
    public Image countdownImage;
    public Sprite[] countdownSprites;
    public float countdownDuration = 3f;
    public AudioSource countdownAudioSource;
    public AudioClip countdownBeep;

    [Header("Pantalla de inicio")]
    public GameObject startScreen;  // Panel que contiene botón y fondo

    [Header("Botón de regreso")]
    public GameObject returnButton;

    [Header("Pantalla final")]
    public GameObject endScreen;
    public TMP_Text finalScoreText;
    public TMP_Text extraFinalText;

    [System.Serializable]
    public class BeatInfo
    {
        public float time;
        public float energy;
    }

    public List<BeatInfo> topBeats = new List<BeatInfo>();

    private float[] spectrum;
    private float averageEnergy = 0f;
    private float lastBeatTime = 0f;

    void Start()
    {
        spectrum = new float[1024];
        countdownImage.gameObject.SetActive(false);

        // Mostrar pantalla de inicio
        if (startScreen != null)
            startScreen.SetActive(true);

        // Ocultar botón de volver al inicio
        if (returnButton != null)
            returnButton.SetActive(false);

        if (endScreen != null)
            endScreen.SetActive(false);
    }
    // Este método se conecta al botón Start
    public void OnStartButtonPressed()
    {
    if (startScreen != null)
        startScreen.SetActive(false); // ocultar pantalla de inicio

    if (returnButton != null)
        returnButton.SetActive(true); // mostrar botón de volver

    gameStarted = true; // el minijuego comienza ahora

    StartCoroutine(StartMusicWithCountdown());
    }

    public void OnReturnButtonPressed()
    {
    Debug.Log("✅ BOTÓN VOLVER PRESIONADO — Volviendo a pantalla de inicio...");
    StopAllCoroutines();

    if (endScreen != null)
        endScreen.SetActive(false); // ocultamos la pantalla final

    if (musicSource != null && musicSource.isPlaying)
        musicSource.Stop();

    foreach (var beat in GameObject.FindGameObjectsWithTag("Beat"))
        Destroy(beat);

    if (returnButton != null)
        returnButton.SetActive(false);

    if (startScreen != null)
        startScreen.SetActive(true);

    topBeats.Clear();
    lastBeatTime = 0f;

    // 🔽 Reiniciar puntuación del gestor de ritmo
    if (RhythmGameManager.instance != null)
        RhythmGameManager.instance.ResetScore();

    gameStarted = false; // el juego se detiene hasta pulsar Start otra vez
    lastBeatTime = 0f;
    topBeats.Clear();
    }

    private System.Collections.IEnumerator StartMusicWithCountdown()
    {
        if (countdownImage != null && countdownSprites.Length > 0)
        {
            countdownImage.gameObject.SetActive(true);

            for (int i = 0; i < countdownDuration; i++)
            {
                int index = Mathf.Clamp(i, 0, countdownSprites.Length - 1);
                countdownImage.sprite = countdownSprites[index];

                if (countdownAudioSource != null && countdownBeep != null)
                    countdownAudioSource.PlayOneShot(countdownBeep);

                yield return new WaitForSeconds(1f);
            }

            // GO! opcional
            if (countdownSprites.Length > countdownDuration)
            {
                countdownImage.sprite = countdownSprites[(int)countdownDuration];
                if (countdownAudioSource != null && countdownBeep != null)
                    countdownAudioSource.PlayOneShot(countdownBeep);
                yield return new WaitForSeconds(0.5f);
            }

            countdownImage.gameObject.SetActive(false);
        }

        if (musicSource != null && musicSource.clip != null)
        {
            musicSource.PlayScheduled(AudioSettings.dspTime + 0.1);

            // Detener detección de beats justo cuando termina el clip
            Invoke(nameof(StopGame), musicSource.clip.length - 0.1f);

            // Mostrar pantalla final con 3 segundos de retraso
            StartCoroutine(ShowEndScreenWithDelay(3f));
        }
    }

    // Corrutina para mostrar la pantalla final con retraso
    private System.Collections.IEnumerator ShowEndScreenWithDelay(float delay)
    {
        // Espera hasta que termine la canción
        yield return new WaitForSeconds(musicSource.clip.length - 0.1f);

        // Espera adicional de 3 segundos
        yield return new WaitForSeconds(delay);

        ShowEndScreen();
    }


    void Update()
    {
        if (!gameStarted) return; // si el juego no empezó, no hacer nada
        if (musicSource == null || !musicSource.isPlaying) return;

        // tu código de detección de beats
        musicSource.GetSpectrumData(spectrum, 0, FFTWindow.Blackman);

        float sum = 0f;
        int lowFreqLimit = spectrum.Length / 8;
        for (int i = 0; i < lowFreqLimit; i++)
            sum += spectrum[i] * spectrum[i];
        float currentEnergy = sum / lowFreqLimit;

        if (currentEnergy > averageEnergy * 2f && musicSource.time - lastBeatTime > 0.3f)
        {
            RegisterBeat(musicSource.time, currentEnergy);
            lastBeatTime = musicSource.time;
        }
        float currentFallSpeed = Mathf.Lerp(initialFallSpeed, maxFallSpeed, musicSource.time / speedIncreaseDuration);

        averageEnergy = Mathf.Lerp(averageEnergy, sum / lowFreqLimit, 0.05f);
    }
    public void OnSalirButtonPressed()
    {
        Debug.Log("⏹ Botón Salir presionado — cambiando de escena a: " + escenaSalir);

        if (!string.IsNullOrEmpty(escenaSalir))
            SceneManager.LoadScene(escenaSalir);
        else
            Debug.LogWarning("⚠️ No se ha asignado ninguna escena en el Inspector.");
    }
    void StopGame()
    {
        gameStarted = false; // 👈 evita que Update siga detectando beats
        Debug.Log("⏹ Detección de beats detenida (final de canción)");
    }

    void ShowEndScreen()
    {
        if (endScreen != null)
            endScreen.SetActive(true);

        if (finalScoreText != null && RhythmGameManager.instance != null)
        {
            int hits = RhythmGameManager.instance.HitNotes;
            int total = RhythmGameManager.instance.TotalNotes;

            finalScoreText.text = $"Has acertat {hits} de {total} notes";

            if (extraFinalText != null)
            {
                float hitRate = (float)hits/total;

                if (hitRate >= 0.95f)
                    extraFinalText.text = "¡Increíble! Eres un maestre del ritme.";
                else if (hitRate >= 0.80f)
                    extraFinalText.text = "¡Molt bé!";
                else if (hitRate >= 0.60f)
                    extraFinalText.text = "Nada mal, pero pots millorar.";
                else
                    extraFinalText.text = "Continua practicant... ";
            }
        }

        if (returnButton != null)
            returnButton.SetActive(false);

        Debug.Log("🎯 Canción terminada. Nota mostrada en pantalla final.");
    }

  void SpawnBeatVisual(float beatEnergy)
{
    if (beatPrefab == null) return;

    float randomX = Random.Range(minX, maxX);
    Vector3 spawnPos = new Vector3(randomX, spawnY, spawnZ);

    GameObject noteObj = Instantiate(beatPrefab, spawnPos, Quaternion.identity);

    // Calcular velocidad base según tiempo de la canción
    float baseSpeed = Mathf.Lerp(initialFallSpeed, maxFallSpeed, musicSource.time / speedIncreaseDuration);

    // Ajustar velocidad según la energía del beat
    float finalSpeed = Mathf.Lerp(initialFallSpeed, maxFallSpeed, musicSource.time / speedIncreaseDuration);

    Note noteScript = noteObj.GetComponent<Note>();
    if (noteScript != null)
    {
        noteScript.fallSpeed = finalSpeed;
    }
}
void RegisterBeat(float time, float energy)
{
    BeatInfo newBeat = new BeatInfo { time = time, energy = energy };
    topBeats.Add(newBeat);
    topBeats.Sort((a, b) => b.energy.CompareTo(a.energy));
    if (topBeats.Count > 50)
        topBeats.RemoveAt(topBeats.Count - 1);

    // Pasar la energía del beat al spawn
    SpawnBeatVisual(energy);

    // Registrar nota en el gestor de ritmo
    if (RhythmGameManager.instance != null)
        RhythmGameManager.instance.RegisterNote();
}


}
