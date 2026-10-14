using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public IntVariable gameScore;

    public UnityEvent gameStart;
    public UnityEvent gameRestart;
    public UnityEvent<int> scoreChange;
    public UnityEvent gameOver;

    public int CurrentScore => gameScore.Value;

    void Start()
    {
        gameScore.Value = 0;

        Time.timeScale = 1.0f;
        gameStart.Invoke();
        SetScore(gameScore.Value);

        SceneManager.activeSceneChanged += SceneSetup;
    }

    public void SceneSetup(Scene current, Scene next)
    {
        gameStart.Invoke();
        SetScore(gameScore.Value);
    }

    void OnDestroy()
    {
        SceneManager.activeSceneChanged -= SceneSetup;
    }

    public void GameRestart()
    {
        gameScore.Value = 0;
        SetScore(gameScore.Value);

        gameRestart.Invoke();
        Time.timeScale = 1.0f;
    }

    public void IncreaseScore(int increment)
    {
        gameScore.ApplyChange(increment);
        SetScore(gameScore.Value);
    }

    public void SetScore(int score)
    {
        scoreChange.Invoke(score);
    }

    public void GameOver()
    {
        Time.timeScale = 0.0f;
        gameOver.Invoke();
    }
}