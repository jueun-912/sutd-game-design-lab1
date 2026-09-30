using UnityEngine;

public class BrickHit : MonoBehaviour
{
    public Animator brickAnimator;
    public Animator coinAnimator;

    public AudioSource coinAudio;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name != "Mario")
            return;

        ContactPoint2D contact = collision.GetContact(0);

        // Activate only when Mario hits the brick from below
        if (contact.normal.y > 0.5f)
        {
            // Bounce the brick once
            brickAnimator.Play("brick-bounce", 0, 0f);

            // Spawn the coin
            coinAnimator.Play("coin-spawn", 0, 0f);
            coinAudio.Play();
        }
    }
}