using UnityEngine;
using UnityEngine.UI;

public class PauseButtonController : MonoBehaviour, IInteractiveButton
{
    private bool isPaused = false;

    public Sprite pauseIcon;
    public Sprite playIcon;

    private Image image;

    void Start()
    {
        image = GetComponent<Image>();

        ResetPause();

        GameManager.instance.gameRestart.AddListener(ResetPause);
    }

    public void ButtonClick()
    {
        isPaused = !isPaused;

        Time.timeScale = isPaused ? 0f : 1f;
        AudioListener.pause = isPaused;

        image.sprite = isPaused ? playIcon : pauseIcon;
    }

    private void ResetPause()
    {
        isPaused = false;

        Time.timeScale = 1f;
        AudioListener.pause = false;

        image.sprite = pauseIcon;
    }

    void OnDestroy()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.gameRestart.RemoveListener(ResetPause);
        }

        AudioListener.pause = false;
    }
}