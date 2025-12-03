using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public PlayerScore2D playerScore;

    void Update()
    {
        if (playerScore.score < 10)
        {
            scoreText.text = "000" + playerScore.score;
        }
        else if(playerScore.score < 100)
        {
            scoreText.text = "00" + playerScore.score;
        }
        else if (playerScore.score < 1000)
        {
            scoreText.text = "0" + playerScore.score;
        }
        else
        {
            scoreText.text = "" + playerScore.score;
        }
    }
}
