using UnityEngine;

public class QuestionBox : MonoBehaviour
{
    public Animator coinAnimator;
    public SpriteRenderer boxSpriteRenderer;
    public Sprite emptySprite;
    public Rigidbody2D boxRigidbody;
    public AudioSource coinAudio;

    private bool used = false;

    private Sprite originalSprite;
    private RigidbodyType2D originalBodyType;
    private Animator boxAnimator;

    void Start()
    {
        originalSprite = boxSpriteRenderer.sprite;
        originalBodyType = boxRigidbody.bodyType;
        boxAnimator = GetComponent<Animator>();

        GameManager.instance.gameRestart.AddListener(GameRestart);
    }

    void OnDestroy()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.gameRestart.RemoveListener(GameRestart);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (used || collision.gameObject.name != "Mario")
        {
            return;
        }

        ContactPoint2D contact = collision.GetContact(0);

        if (contact.normal.y > 0.5f)
        {
            used = true;

            coinAnimator.Play("coin-spawn", 0, 0f);
            coinAudio.Play();

            if (boxAnimator != null)
            {
                boxAnimator.enabled = false;
            }

            boxSpriteRenderer.sprite = emptySprite;

            boxRigidbody.linearVelocity = Vector2.zero;
            boxRigidbody.bodyType = RigidbodyType2D.Static;
        }
    }

    public void GameRestart()
    {
        used = false;

        boxSpriteRenderer.sprite = originalSprite;

        boxRigidbody.bodyType = originalBodyType;
        boxRigidbody.linearVelocity = Vector2.zero;

        if (boxAnimator != null)
        {
            boxAnimator.enabled = true;
            boxAnimator.Play(0, 0, 0f);
        }

        coinAnimator.Rebind();
        coinAnimator.Update(0f);
    }
}