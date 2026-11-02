using UnityEngine;

public class GameManager : MonoBehaviour
{
    public IntVariable gameScore;

    public SimpleGameEvent onGameStart;
    public IntGameEvent onUpdateScore;

    public int CurrentScore => gameScore.Value;

    void Start()
    {
        Time.timeScale = 1f;

        onGameStart.Raise();
        SetScore(gameScore.Value);
    }

    public void GameRestart()
    {
        Time.timeScale = 1f;

        gameScore.Value = 0;
        SetScore(gameScore.Value);
    }

    public void IncreaseScore(int increment)
    {
        gameScore.ApplyChange(increment);
        SetScore(gameScore.Value);
    }

    public void SetScore(int score)
    {
        onUpdateScore.Raise(score);
    }

    public void GameOver()
    {
        Time.timeScale = 0f;
    }
}