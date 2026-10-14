using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RestartMusic : MonoBehaviour
{
    private AudioSource music;

    void Start()
    {
        music = GetComponent<AudioSource>();
        GameManager.instance.gameRestart.AddListener(RestartAudio);
    }

    void RestartAudio()
    {
        AudioListener.pause = false;
        music.Stop();
        music.Play();
    }

    void OnDestroy()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.gameRestart.RemoveListener(RestartAudio);
        }
    }
}