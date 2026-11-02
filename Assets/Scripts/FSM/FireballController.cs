using UnityEngine;

public class FireballController : MonoBehaviour
{
    public float lifetime = 2.5f;

    private bool used;

    private void Start()
    {
        Destroy(gameObject, lifetime);

        AudioSource audio = GetComponent<AudioSource>();

        if (audio != null && audio.clip != null)
            audio.Play();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (used)
            return;

        if (other.CompareTag("Player"))
            return;

        if (other.attachedRigidbody != null &&
            other.attachedRigidbody.CompareTag("Player"))
        {
            return;
        }

        GoombaStomp goomba =
            other.GetComponentInParent<GoombaStomp>();

        if (goomba != null)
        {
            used = true;
            goomba.Stomp();
            Destroy(gameObject);
            return;
        }

        // 다른 Trigger는 통과하고, 벽이나 바닥에서는 제거
        if (!other.isTrigger)
        {
            used = true;
            Destroy(gameObject);
        }
    }

    public void GameRestart()
    {
        Destroy(gameObject);
    }
}