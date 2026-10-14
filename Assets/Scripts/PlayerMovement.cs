using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, IPowerupApplicable
{
    public GameConstants gameConstants;

    private float upSpeed;
    private float maxSpeed;
    private float speed;
    private float deathImpulse;

    private bool onGroundState = true;
    private bool faceRightState = true;
    private bool moving = false;
    private bool jumpedState = false;

    private Rigidbody2D marioBody;
    private SpriteRenderer marioSprite;
    private Collider2D marioCollider;

    public AudioClip marioDeath;

    [System.NonSerialized]
    public bool alive = true;

    private Vector3 marioStartPosition;
    private Vector3 cameraStartPosition;
    private Vector3 originalScale;

    private bool isSuper;
    private bool invincible;

    public Animator marioAnimator;
    public AudioSource marioAudio;
    public AudioSource marioDeathAudio;
    public Transform gameCamera;

    private int collisionLayerMask =
        (1 << 3) | (1 << 6) | (1 << 7);

    void Awake()
    {
        GameManager.instance.gameRestart.AddListener(GameRestart);
    }

    void Start()
    {
        speed = gameConstants.speed;
        maxSpeed = gameConstants.maxSpeed;
        deathImpulse = gameConstants.deathImpulse;
        upSpeed = gameConstants.upSpeed;

        Application.targetFrameRate = 30;

        marioBody = GetComponent<Rigidbody2D>();
        marioSprite = GetComponent<SpriteRenderer>();
        marioCollider = GetComponent<Collider2D>();

        marioStartPosition = transform.position;
        originalScale = transform.localScale;

        if (Camera.main != null)
        {
            gameCamera = Camera.main.transform;
        }

        if (gameCamera != null)
        {
            cameraStartPosition = gameCamera.position;
        }

        marioAnimator.SetBool("onGround", onGroundState);
    }

    void OnDestroy()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.gameRestart.RemoveListener(GameRestart);
        }
    }

    void Update()
    {
        marioAnimator.SetFloat(
            "xSpeed",
            Mathf.Abs(marioBody.linearVelocity.x)
        );
    }

    void LateUpdate()
    {
        // AnimatorがScaleを変更してもパワーアップのサイズを保つ
        transform.localScale =
            originalScale * (isSuper ? 1.5f : 1f);
    }

    public void RequestPowerupEffect(IPowerup powerup)
    {
        if (!alive)
            return;

        if (powerup.powerupType == PowerupType.MagicMushroom)
        {
            isSuper = true;
            transform.localScale = originalScale * 1.5f;

            Debug.Log("Magic Mushroom applied!");
        }
    }

    void FlipMarioSprite(int value)
    {
        if (value == -1 && faceRightState)
        {
            faceRightState = false;
            marioSprite.flipX = true;

            if (marioBody.linearVelocity.x > 0.05f)
            {
                marioAnimator.SetTrigger("onSkid");
            }
        }
        else if (value == 1 && !faceRightState)
        {
            faceRightState = true;
            marioSprite.flipX = false;

            if (marioBody.linearVelocity.x < -0.05f)
            {
                marioAnimator.SetTrigger("onSkid");
            }
        }
    }

    void FixedUpdate()
    {
        if (alive && moving)
        {
            Move(faceRightState ? 1 : -1);
        }
    }

    void Move(int value)
    {
        Vector2 movement = new Vector2(value, 0);

        if (marioBody.linearVelocity.magnitude < maxSpeed)
        {
            marioBody.AddForce(movement * speed);
        }
    }

    public void MoveCheck(int value)
    {
        if (value == 0 || !alive)
        {
            moving = false;
        }
        else
        {
            FlipMarioSprite(value);
            moving = true;
            Move(value);
        }
    }

    public void Jump()
    {
        if (alive && onGroundState)
        {
            marioBody.AddForce(
                Vector2.up * upSpeed,
                ForceMode2D.Impulse
            );

            onGroundState = false;
            jumpedState = true;

            marioAnimator.SetBool("onGround", onGroundState);
        }
    }

    public void JumpHold()
    {
        if (alive && jumpedState)
        {
            marioBody.AddForce(
                Vector2.up * upSpeed * 30,
                ForceMode2D.Force
            );

            jumpedState = false;
        }
    }

    void PlayDeathImpulse()
    {
        marioBody.AddForce(
            Vector2.up * deathImpulse,
            ForceMode2D.Impulse
        );
    }

    public void GameOverScene()
    {
        GameManager.instance.GameOver();
    }

    void PlayJumpSound()
    {
        marioAudio.PlayOneShot(marioAudio.clip);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (((collisionLayerMask &
             (1 << col.transform.gameObject.layer)) > 0)
            && !onGroundState)
        {
            onGroundState = true;
            jumpedState = false;

            marioAnimator.SetBool("onGround", onGroundState);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Enemy") || !alive)
            return;

        GoombaStomp goomba = other.GetComponent<GoombaStomp>();

        bool isFalling = marioBody.linearVelocity.y < 0f;

        bool isAbove =
            marioCollider.bounds.min.y >= other.bounds.center.y;

        if (goomba != null && isFalling && isAbove)
        {
            goomba.Stomp();
            jumpedState = false;

            marioBody.linearVelocity =
                new Vector2(marioBody.linearVelocity.x, 0f);

            marioBody.AddForce(
                Vector2.up * upSpeed * 0.5f,
                ForceMode2D.Impulse
            );

            return;
        }

        if (invincible)
            return;

        // Super Marioなら一度だけ攻撃を耐える
        if (isSuper)
        {
            isSuper = false;
            transform.localScale = originalScale;

            StartCoroutine(TemporaryInvincibility());
            return;
        }

        marioAnimator.Play("mario-die");

        if (marioDeathAudio != null)
        {
            marioDeathAudio.PlayOneShot(marioDeath);
        }
        else
        {
            marioAudio.PlayOneShot(marioDeath);
        }

        moving = false;
        alive = false;
    }

    private IEnumerator TemporaryInvincibility()
    {
        invincible = true;

        for (int i = 0; i < 10; i++)
        {
            marioSprite.enabled = !marioSprite.enabled;
            yield return new WaitForSeconds(0.1f);
        }

        marioSprite.enabled = true;
        invincible = false;
    }

    public void GameRestart()
    {
        StopAllCoroutines();

        isSuper = false;
        invincible = false;

        transform.localScale = originalScale;
        marioSprite.enabled = true;

        marioBody.transform.position = marioStartPosition;
        marioBody.linearVelocity = Vector2.zero;

        onGroundState = true;
        jumpedState = false;
        moving = false;

        faceRightState = true;
        marioSprite.flipX = false;

        marioAnimator.SetBool("onGround", onGroundState);
        marioAnimator.SetTrigger("gameRestart");

        alive = true;

        if (gameCamera != null)
        {
            gameCamera.position = cameraStartPosition;
        }
    }
}