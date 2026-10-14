using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadAndWait : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    void Start()
    {
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        yield return new WaitForSecondsRealtime(1f);

        for (float alpha = 1f; alpha >= 0f; alpha -= 0.05f)
        {
            canvasGroup.alpha = alpha;
            yield return new WaitForSecondsRealtime(0.05f);
        }

        SceneManager.LoadScene("MarioScene");
    }

    public void ReturnToMain()
    {
        SceneManager.LoadScene("MainMenu");
    }
}