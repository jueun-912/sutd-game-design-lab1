using UnityEngine;
using UnityEngine.Events;

public class RestartButtonController : MonoBehaviour
{
    public UnityEvent gameRestart = new UnityEvent();

    public void ButtonClick()
    {
        gameRestart.Invoke();
    }
}