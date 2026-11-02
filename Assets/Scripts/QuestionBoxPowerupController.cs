using UnityEngine;

public class QuestionBoxPowerupController
    : MonoBehaviour, IPowerupController
{
    public Animator powerupAnimator;
    public BasePowerup powerup;
    public AudioSource spawnAudio;

    private Animator boxAnimator;
    private Rigidbody2D boxBody;

    private Vector3 startingPosition;
    private RigidbodyType2D startingBodyType;
    private bool activated;

    void Start()
    {
        boxAnimator = GetComponent<Animator>();
        boxBody = GetComponent<Rigidbody2D>();

        startingPosition = transform.localPosition;
        startingBodyType = boxBody.bodyType;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (activated || powerup.hasSpawned)
            return;

        PlayerMovement player =
            other.gameObject.GetComponentInParent<PlayerMovement>();

        if (player == null || !player.alive)
            return;

        bool hitFromBelow = false;

        for (int i = 0; i < other.contactCount; i++)
        {
            if (other.GetContact(i).normal.y > 0.5f)
            {
                hitFromBelow = true;
                break;
            }
        }

        if (!hitFromBelow)
            return;

        activated = true;

        boxAnimator.SetTrigger("spawned");
        powerupAnimator.SetTrigger("spawned");

        if (spawnAudio != null)
        {
            spawnAudio.Play();
        }
    }

    public void Disable()
    {
        boxBody.linearVelocity = Vector2.zero;
        boxBody.angularVelocity = 0f;
        boxBody.bodyType = RigidbodyType2D.Static;

        transform.localPosition = startingPosition;
    }

    public void GameRestart()
    {
        activated = false;

        boxBody.bodyType = startingBodyType;
        boxBody.linearVelocity = Vector2.zero;
        boxBody.angularVelocity = 0f;

        boxAnimator.enabled = true;
        boxAnimator.Rebind();
        boxAnimator.Update(0f);

        transform.localPosition = startingPosition;
    }
}