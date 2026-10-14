using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GoombaStomp : MonoBehaviour
{
    public UnityEvent<int> stomped;
    public int scoreValue = 1;

    public AudioSource stompAudio;

    private Vector3 startScale;

    private Collider2D goombaCollider;
    private EnemyMovement enemyMovement;
    private SpriteRenderer spriteRenderer;

    private bool isStomped = false;

    void Awake()
    {
        startScale = transform.localScale;

        goombaCollider = GetComponent<Collider2D>();
        enemyMovement = GetComponent<EnemyMovement>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Stomp()
    {
        if (isStomped)
            return;

        isStomped = true;

        // Stop moving and disable collision
        enemyMovement.enabled = false;
        goombaCollider.enabled = false;

        // Increase score
        stomped.Invoke(scoreValue);

        // Play stomp sound
        if (stompAudio != null)
        {
            stompAudio.Play();
        }

        // Squish and disappear
        StartCoroutine(SquishAndDisappear());
    }

    IEnumerator SquishAndDisappear()
    {
        Vector3 targetScale = new Vector3(
            startScale.x,
            startScale.y * 0.25f,
            startScale.z
        );

        float duration = 0.12f;
        float elapsed = 0f;

        // Squish animation
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            transform.localScale = Vector3.Lerp(
                startScale,
                targetScale,
                elapsed / duration
            );

            yield return null;
        }

        transform.localScale = targetScale;

        // Stay squished for a moment
        yield return new WaitForSeconds(0.3f);

        // Disappear
        spriteRenderer.enabled = false;
    }

    public void GameRestart()
    {
        StopAllCoroutines();

        isStomped = false;

        // Restore appearance
        transform.localScale = startScale;
        spriteRenderer.enabled = true;

        // Restore movement and collision
        enemyMovement.enabled = true;
        goombaCollider.enabled = true;
    }
}