using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenuController : MonoBehaviour
{
    public IntVariable gameScore;

    public void StartGame()
    {
        gameScore.Value = 0;
        Time.timeScale = 1f;
        SceneManager.LoadScene("LoadingScene");
    }

    public void ResetHighscore()
    {
        gameScore.ResetHighestValue();

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        Debug.Log("Highscore reset: 0");
    }
}
