using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public float upSpeed = 20;
    private bool onGroundState = true;

    public float maxSpeed = 30;
    public float speed = 15;

    private Rigidbody2D marioBody;
    private SpriteRenderer marioSprite;
    private bool faceRightState = true;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gameOverScoreText;

    public GameObject enemies;
    public GameObject hud;
    public GameObject gameOverPanel;

    public AudioClip marioDeath;
    public float deathImpulse = 15;

    [System.NonSerialized]
    public bool alive = true;

    private Vector3 marioStartPosition;

    public JumpOverGoomba jumpOverGoomba;

    // for animation
    public Animator marioAnimator;

    // for audio
    public AudioSource marioAudio;

    // for camera
    public Transform gameCamera;

    // Ground + Enemies + Obstacles
    int collisionLayerMask = (1 << 3) | (1 << 6) | (1 << 7);

    void Start()
    {
        Application.targetFrameRate = 30;

        marioBody = GetComponent<Rigidbody2D>();
        marioSprite = GetComponent<SpriteRenderer>();

        // save Mario's starting position
        marioStartPosition = transform.position;

        hud.SetActive(true);
        gameOverPanel.SetActive(false);

        // update animator state
        marioAnimator.SetBool("onGround", onGroundState);
    }

    void Update()
    {
        if (Input.GetKeyDown("a") && faceRightState)
        {
            faceRightState = false;
            marioSprite.flipX = true;

            if (marioBody.linearVelocity.x > 0.1f)
                marioAnimator.SetTrigger("onSkid");
        }

        if (Input.GetKeyDown("d") && !faceRightState)
        {
            faceRightState = true;
            marioSprite.flipX = false;

            if (marioBody.linearVelocity.x < -0.1f)
                marioAnimator.SetTrigger("onSkid");
        }

        marioAnimator.SetFloat(
            "xSpeed",
            Mathf.Abs(marioBody.linearVelocity.x)
        );
    }

    void PlayDeathImpulse()
    {
        marioBody.AddForce(
            Vector2.up * deathImpulse,
            ForceMode2D.Impulse
        );
    }

    void GameOverScene()
    {
        gameOverScoreText.text =
            "Score: " + jumpOverGoomba.score.ToString();

        hud.SetActive(false);
        gameOverPanel.SetActive(true);

        Time.timeScale = 0.0f;
    }

    void PlayJumpSound()
    {
        marioAudio.PlayOneShot(marioAudio.clip);
    }

    void FixedUpdate()
    {
        if (alive)
        {
            float moveHorizontal = Input.GetAxisRaw("Horizontal");

            if (Mathf.Abs(moveHorizontal) > 0)
            {
                Vector2 movement =
                    new Vector2(moveHorizontal, 0);

                if (marioBody.linearVelocity.magnitude < maxSpeed)
                    marioBody.AddForce(movement * speed);
            }

            if (Input.GetKeyUp("a") || Input.GetKeyUp("d"))
            {
                marioBody.linearVelocity = Vector2.zero;
            }

            if (Input.GetKeyDown("space") && onGroundState)
            {
                marioBody.AddForce(
                    Vector2.up * upSpeed,
                    ForceMode2D.Impulse
                );

                onGroundState = false;

                // update animator state
                marioAnimator.SetBool(
                    "onGround",
                    onGroundState
                );
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (((collisionLayerMask &
             (1 << col.transform.gameObject.layer)) > 0)
             && !onGroundState)
        {
            onGroundState = true;

            marioAnimator.SetBool(
                "onGround",
                onGroundState
            );
        }
    }

    public void RestartButtonCallback(int input)
    {
        Debug.Log("Restart!");

        ResetGame();

        gameOverPanel.SetActive(false);
        hud.SetActive(true);

        Time.timeScale = 1.0f;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy") && alive)
        {
            Debug.Log("Collided with goomba!");

            marioAnimator.Play("mario-die");
            marioAudio.PlayOneShot(marioDeath);
            alive = false;
        }
    }
    private void ResetGame()
    {
        marioBody.transform.position = marioStartPosition;
        marioBody.linearVelocity = Vector2.zero;

        onGroundState = true;

        faceRightState = true;
        marioSprite.flipX = false;

        scoreText.text = "Score: 0";

        foreach (Transform eachChild in enemies.transform)
        {
            eachChild.transform.localPosition =
                eachChild.GetComponent<EnemyMovement>().startPosition;
        }

        jumpOverGoomba.score = 0;

        marioAnimator.SetTrigger("gameRestart");
        alive = true;

        gameCamera.position = new Vector3(0, 0, -10);
    }
}