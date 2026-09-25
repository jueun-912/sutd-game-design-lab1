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

    private Vector3 marioStartPosition;

    public JumpOverGoomba jumpOverGoomba;

    void Start()
    {
        Application.targetFrameRate = 30;

        marioBody = GetComponent<Rigidbody2D>();
        marioSprite = GetComponent<SpriteRenderer>();

        // save Mario's starting position
        marioStartPosition = transform.position;

        hud.SetActive(true);
        gameOverPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown("a") && faceRightState)
        {
            faceRightState = false;
            marioSprite.flipX = true;
        }

        if (Input.GetKeyDown("d") && !faceRightState)
        {
            faceRightState = true;
            marioSprite.flipX = false;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
            onGroundState = true;
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(moveHorizontal) > 0)
        {
            Vector2 movement = new Vector2(moveHorizontal, 0);

            if (marioBody.linearVelocity.magnitude < maxSpeed)
                marioBody.AddForce(movement * speed);
        }

        if (Input.GetKeyUp("a") || Input.GetKeyUp("d"))
        {
            marioBody.linearVelocity = Vector2.zero;
        }

        if (Input.GetKeyDown("space") && onGroundState)
        {
            marioBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
            onGroundState = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Collided with goomba!");

            gameOverScoreText.text =
                "Score: " + jumpOverGoomba.score.ToString();

            hud.SetActive(false);
            gameOverPanel.SetActive(true);

            Time.timeScale = 0.0f;
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

    private void ResetGame()
    {
        // reset Mario position
        marioBody.transform.position = marioStartPosition;

        // reset Mario velocity
        marioBody.linearVelocity = Vector2.zero;

        // reset ground state
        onGroundState = true;

        // reset sprite direction
        faceRightState = true;
        marioSprite.flipX = false;

        // reset score text
        scoreText.text = "Score: 0";

        // reset Goomba
        foreach (Transform eachChild in enemies.transform)
        {
            eachChild.transform.localPosition =
                eachChild.GetComponent<EnemyMovement>().startPosition;
        }

        jumpOverGoomba.score = 0;
    }
}