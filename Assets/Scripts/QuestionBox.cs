using UnityEngine;

public class QuestionBox : MonoBehaviour
{
    public Animator coinAnimator;
    public SpriteRenderer boxSpriteRenderer;
    public Sprite emptySprite;
    public Rigidbody2D boxRigidbody;
    public AudioSource coinAudio;

    private bool used = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (used || collision.gameObject.name != "Mario")
            return;

        ContactPoint2D contact = collision.GetContact(0);

        // Activate only when Mario hits the box from below
        if (contact.normal.y > 0.5f)
        {
            used = true;

            // Play the coin spawn animation
            coinAnimator.Play("coin-spawn", 0, 0f);

            // Play the coin sound
            coinAudio.Play();

            // Stop the blinking animation
            Animator boxAnimator = GetComponent<Animator>();
            if (boxAnimator != null)
                boxAnimator.enabled = false;

            // Change to the empty question box immediately
            boxSpriteRenderer.sprite = emptySprite;

            // Disable the spring
            boxRigidbody.linearVelocity = Vector2.zero;
            boxRigidbody.bodyType = RigidbodyType2D.Static;
        }
    }
}