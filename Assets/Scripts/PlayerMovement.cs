using UnityEngine;

public class PlayerMovement : MonoBehaviour, IPowerupApplicable
{
    public GameConstants gameConstants;
    public SimpleGameEvent onGameOver;

    public Animator marioAnimator;
    public AudioSource marioAudio;
    public AudioSource marioDeathAudio;
    public AudioClip marioDeath;

    [System.NonSerialized]
    public bool alive = true;

    private Rigidbody2D marioBody;
    private SpriteRenderer marioSprite;
    private Collider2D marioCollider;

    private MarioStateController marioStates;
    private BuffStateController buffs;

    private Vector3 startPosition;
    private Vector3 originalScale;
    private Color originalColor;

    public Vector3 OriginalScale => originalScale;

    private bool onGround = true;
    private bool faceRight = true;
    private bool moving;
    private bool jumped;
    private bool deathStarted;
    private bool gameOverRaised;

    private void Awake()
    {
        marioBody = GetComponent<Rigidbody2D>();
        marioSprite = GetComponent<SpriteRenderer>();
        marioCollider = GetComponent<Collider2D>();

        marioStates = GetComponent<MarioStateController>();
        buffs = GetComponent<BuffStateController>();

        if (marioAnimator == null)
            marioAnimator = GetComponent<Animator>();

        if (marioAudio == null)
            marioAudio = GetComponent<AudioSource>();

        startPosition = transform.position;
        originalScale = transform.localScale;
        originalColor = marioSprite.color;
    }

    private void Start()
    {
        Application.targetFrameRate = 30;
        RefreshAnimationState();
    }

    private void Update()
    {
        if (!alive)
        {
            if (!deathStarted)
                BeginDeath();

            return;
        }

        RefreshAnimationState();
    }

    public void RefreshAnimationState()
    {
        marioAnimator.SetBool("onGround", onGround);

        marioAnimator.SetFloat(
            "xSpeed",
            Mathf.Abs(marioBody.linearVelocity.x)
        );
    }

    private void LateUpdate()
    {
        float multiplier = 1f;

        if (marioStates != null)
        {
            MarioState state = marioStates.currentMarioState;

            if (state == MarioState.SuperMario ||
                state == MarioState.FireMario)
            {
                multiplier = 0.75f;
            }
        }

        // 애니메이션이 Scale을 바꿔도 상태별 크기를 유지
        transform.localScale = originalScale * multiplier;

        if (alive && buffs != null && buffs.IsInvincible)
        {
            marioSprite.color = Color.HSVToRGB(
                Mathf.Repeat(Time.time * 1.5f, 1f),
                0.8f,
                1f
            );
        }
        else
        {
            marioSprite.color = originalColor;
        }
    }

    private void FixedUpdate()
    {
        if (alive && moving)
            Move(faceRight ? 1 : -1);
    }

    private void Move(int direction)
    {
        if (Mathf.Abs(marioBody.linearVelocity.x)
            < gameConstants.maxSpeed)
        {
            marioBody.AddForce(
                Vector2.right * direction * gameConstants.speed
            );
        }
    }

    public void MoveCheck(int value)
    {
        moving = value != 0 && alive;

        if (!moving)
            return;

        bool nextFaceRight = value > 0;

        if (nextFaceRight != faceRight &&
            Mathf.Abs(marioBody.linearVelocity.x) > 0.05f)
        {
            marioAnimator.SetTrigger("onSkid");
        }

        faceRight = nextFaceRight;
        marioSprite.flipX = !faceRight;

        Move(faceRight ? 1 : -1);
    }

    public void Jump()
    {
        if (!alive || !onGround)
            return;

        marioBody.AddForce(
            Vector2.up * gameConstants.upSpeed,
            ForceMode2D.Impulse
        );

        onGround = false;
        jumped = true;

        marioAnimator.SetBool("onGround", false);
    }

    public void JumpHold()
    {
        if (!alive || !jumped)
            return;

        marioBody.AddForce(
            Vector2.up * gameConstants.upSpeed * 30f,
            ForceMode2D.Force
        );

        jumped = false;
    }

    public void RequestPowerupEffect(IPowerup powerup)
    {
        if (!alive || powerup == null)
            return;

        if ((int)powerup.powerupType == 3)
        {
            buffs.CollectStar();
        }
        else
        {
            marioStates.SetPowerup(powerup.powerupType);
        }
    }

    public void DamageMario()
    {
        if (!alive || buffs.IsInvincible)
            return;

        if (marioStates.currentMarioState ==
            MarioState.InvincibleSmallMario)
        {
            return;
        }

        marioStates.SetPowerup((PowerupType)99);

        if (marioStates.currentState != null)
            marioStates.currentState.UpdateState(marioStates);
    }

    private void BeginDeath()
    {
        deathStarted = true;
        moving = false;

        marioBody.linearVelocity = Vector2.zero;
        marioCollider.enabled = false;

        AudioSource source = marioDeathAudio != null
            ? marioDeathAudio
            : marioAudio;

        if (source != null && marioDeath != null)
            source.PlayOneShot(marioDeath);
    }

    public void PlayDeathImpulse()
    {
        if (alive)
            return;

        marioBody.AddForce(
            Vector2.up * gameConstants.deathImpulse,
            ForceMode2D.Impulse
        );
    }

    public void GameOverScene()
    {
        if (alive || gameOverRaised)
            return;

        gameOverRaised = true;
        onGameOver.Raise();
    }

    public void PlayJumpSound()
    {
        if (marioAudio != null && marioAudio.clip != null)
            marioAudio.PlayOneShot(marioAudio.clip);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            if (collision.GetContact(i).normal.y > 0.5f)
            {
                onGround = true;
                jumped = false;

                if (alive)
                    marioAnimator.SetBool("onGround", true);

                break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!alive || !other.CompareTag("Enemy"))
            return;

        GoombaStomp goomba =
            other.GetComponentInParent<GoombaStomp>();

        if (goomba == null)
            return;

        if (buffs.IsInvincible)
        {
            goomba.Stomp();
            return;
        }

        bool falling = marioBody.linearVelocity.y < 0f;

        bool above =
            marioCollider.bounds.min.y >= other.bounds.center.y;

        if (falling && above)
        {
            goomba.Stomp();

            jumped = false;

            marioBody.linearVelocity = new Vector2(
                marioBody.linearVelocity.x,
                0f
            );

            marioBody.AddForce(
                Vector2.up * gameConstants.upSpeed * 0.5f,
                ForceMode2D.Impulse
            );

            return;
        }

        DamageMario();
    }

    public void GameRestart()
    {
        StopAllCoroutines();

        alive = true;
        moving = false;
        jumped = false;
        onGround = true;
        faceRight = true;
        deathStarted = false;
        gameOverRaised = false;

        marioCollider.enabled = true;
        marioSprite.enabled = true;
        marioSprite.flipX = false;
        marioSprite.color = originalColor;

        transform.localScale = originalScale;

        if (marioAudio != null)
            marioAudio.Stop();

        if (marioDeathAudio != null)
            marioDeathAudio.Stop();

        // 상태와 외형부터 초기화
        marioStates.GameRestart();
        buffs.GameRestart();

        marioAnimator.Rebind();
        marioAnimator.Update(0f);

        // 모든 상태 초기화가 끝난 뒤 위치 복원
        transform.localScale = originalScale;
        transform.position = startPosition;

        marioBody.position = new Vector2(
            startPosition.x,
            startPosition.y
        );

        marioBody.linearVelocity = Vector2.zero;
        marioBody.angularVelocity = 0f;

        RefreshAnimationState();
        Physics2D.SyncTransforms();
    }
}