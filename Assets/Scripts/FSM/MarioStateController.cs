using System.Collections;
using UnityEngine;

public class MarioStateController : StateController
{
    public PowerupType currentPowerupType = (PowerupType)(-1);
    public MarioState currentMarioState = MarioState.SmallMario;

    public float flickerInterval = 0.1f;

    private SpriteRenderer marioSprite;
    private Coroutine flickerCoroutine;

    private void Awake()
    {
        marioSprite = GetComponent<SpriteRenderer>();
    }

    public override void Start()
    {
        GameRestart();
    }

    public void SetPowerup(PowerupType powerup)
    {
        currentPowerupType = powerup;
    }

    public void ClearPowerup()
    {
        currentPowerupType = (PowerupType)(-1);
    }

    public void Fire()
    {
        if (!isActive || Time.timeScale == 0f || currentState == null)
            return;

        currentState.DoEventTriggeredActions(this, ActionType.Attack);
    }

    public void SetRendererToFlicker()
    {
        StopFlickering();
        flickerCoroutine = StartCoroutine(BlinkSpriteRenderer());
    }

    private IEnumerator BlinkSpriteRenderer()
    {
        while (currentMarioState == MarioState.InvincibleSmallMario)
        {
            marioSprite.enabled = !marioSprite.enabled;
            yield return new WaitForSeconds(flickerInterval);
        }

        marioSprite.enabled = true;
        flickerCoroutine = null;
    }

    public void StopFlickering()
    {
        if (flickerCoroutine != null)
        {
            StopCoroutine(flickerCoroutine);
            flickerCoroutine = null;
        }

        if (marioSprite != null)
            marioSprite.enabled = true;
    }

    public void GameRestart()
    {
        StopFlickering();

        isActive = true;
        currentMarioState = MarioState.SmallMario;

        ClearPowerup();
        ResetStateMachine();
    }

    private void OnDisable()
    {
        StopFlickering();
    }
}