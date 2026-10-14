using UnityEngine;
using UnityEngine.Events;

public class AnimationEventIntTool : MonoBehaviour
{
    public int parameter;
    public UnityEvent<int> useInt;

    public void TriggerIntEvent()
    {
        useInt.Invoke(parameter);
    }

    public void IncreaseScore(int amount)
    {
        GameManager.instance.IncreaseScore(amount);
    }
}