using UnityEngine;
using TMPro;

public class RhythmGameManager : MonoBehaviour
{
    public static RhythmGameManager instance;
    public TMP_Text scoreText;
    private int currentScore = 0;
    private int hitNotes;
    private int totalNotes = 0;
    public int HitNotes => hitNotes;
    public int TotalNotes => totalNotes;

    void Awake()
    {
        instance = this;
    }

    public void RegisterHit()
    {
        hitNotes++;
    }

    public void NoteHit()
    {
        hitNotes++;
        currentScore++;

        if (scoreText != null)
            scoreText.text = "Punts: " + currentScore;
    }

    public void RegisterNote()
    {
        totalNotes++;
    }

    public float GetHitRate()
    {
        if (totalNotes == 0) return 0f;
        return (float)hitNotes / totalNotes * 10f;
    }

    public void ResetScore()
    {
        currentScore = 0;
        totalNotes = 0;

        if (scoreText != null)
            scoreText.text = "Punts: 0";
    }

    public void MissNote()
    {
        // Resta un punto si no se presiona ninguna nota válida
        currentScore--;
        if (currentScore < 0) currentScore = 0;  // No dejar que el puntaje sea negativo

        if (scoreText != null)
            scoreText.text = "Punts: " + currentScore;
    }

    public float GetAccuracy()
    {
        if (totalNotes == 0) return 0f;
        return (float)currentScore / totalNotes * 100f;
    }
}
