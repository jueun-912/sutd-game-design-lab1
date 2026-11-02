using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gameOverScoreText;

    public GameObject highscoreText;
    public IntVariable gameScore;

    public GameObject hud;
    public GameObject gameOverPanel;

    public SimpleGameEvent onGameRestart;

    void Start()
    {
        GameStart();
        SetScore(gameScore.Value);
    }

    public void GameStart()
    {
        hud.SetActive(true);
        gameOverPanel.SetActive(false);
        highscoreText.SetActive(false);
    }

    public void SetScore(int score)
    {
        scoreText.text = "SCORE: " + score.ToString();
        gameOverScoreText.text = "SCORE: " + score.ToString();
    }

    public void GameOver()
    {
        highscoreText.GetComponent<TextMeshProUGUI>().text =
            "TOP- " + gameScore.previousHighestValue.ToString("D6");

        highscoreText.SetActive(true);

        hud.SetActive(false);
        gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        onGameRestart.Raise();
    }
}