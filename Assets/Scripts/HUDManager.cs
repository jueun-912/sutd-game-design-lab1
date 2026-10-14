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

    void Awake()
    {
        GameManager.instance.gameStart.AddListener(GameStart);
        GameManager.instance.gameOver.AddListener(GameOver);
        GameManager.instance.gameRestart.AddListener(GameStart);
        GameManager.instance.scoreChange.AddListener(SetScore);
    }

    void Start()
    {
        GameStart();
        SetScore(GameManager.instance.CurrentScore);
    }

    void OnDestroy()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.gameStart.RemoveListener(GameStart);
            GameManager.instance.gameOver.RemoveListener(GameOver);
            GameManager.instance.gameRestart.RemoveListener(GameStart);
            GameManager.instance.scoreChange.RemoveListener(SetScore);
        }
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
        GameManager.instance.GameRestart();
    }
}