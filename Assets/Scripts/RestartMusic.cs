using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RestartMusic : MonoBehaviour
{
    private AudioSource music;

    void Awake()
    {
        music = GetComponent<AudioSource>();
    }

    public void RestartAudio()
    {
        AudioListener.pause = false;
        music.Stop();
        music.Play();
    }
}