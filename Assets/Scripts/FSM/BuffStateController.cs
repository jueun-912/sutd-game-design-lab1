using UnityEngine;

public class BuffStateController : StateController
{
    public bool starCollected;

    public bool IsInvincible { get; private set; }

    public override void Start()
    {
        GameRestart();
    }

    public void CollectStar()
    {
        if (IsInvincible)
        {
            stateTimeElapsed = 0f;
            return;
        }

        starCollected = true;
    }

    public void StartInvincibility()
    {
        IsInvincible = true;
        starCollected = false;
    }

    public void ClearBuff()
    {
        IsInvincible = false;
        starCollected = false;
    }

    public void GameRestart()
    {
        isActive = true;
        ClearBuff();
        ResetStateMachine();
    }
}