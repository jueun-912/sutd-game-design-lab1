using System.Collections;
using UnityEngine;

public class MagicMushroomPowerup : BasePowerup
{
    public PowerupGameEvent onPowerupCollected;

    private Collider2D mushroomCollider;
    private SpriteRenderer mushroomRenderer;
    private Animator mushroomAnimator;
    private Vector3 startingPosition;
    private bool readyToCollect;

    protected override void Start()
    {
        base.Start();

        type = PowerupType.MagicMushroom;

        mushroomCollider = GetComponent<Collider2D>();
        mushroomRenderer = GetComponent<SpriteRenderer>();
        mushroomAnimator = GetComponent<Animator>();

        startingPosition = transform.localPosition;

        PrepareHidden();
    }

    private void PrepareHidden()
    {
        spawned = false;
        consumed = false;
        goRight = true;
        readyToCollect = false;

        mushroomCollider.enabled = false;

        rigidBody.simulated = true;
        rigidBody.bodyType = RigidbodyType2D.Dynamic;
        rigidBody.linearVelocity = Vector2.zero;
        rigidBody.angularVelocity = 0f;
        rigidBody.gravityScale = 0f;
        rigidBody.bodyType = RigidbodyType2D.Static;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        TryCollect(col.gameObject);

        if (!readyToCollect || consumed)
            return;

        for (int i = 0; i < col.contactCount; i++)
        {
            if (Mathf.Abs(col.GetContact(i).normal.x) > 0.5f)
            {
                goRight = !goRight;

                rigidBody.linearVelocity = new Vector2(
                    goRight ? 3f : -3f,
                    rigidBody.linearVelocity.y
                );

                break;
            }
        }
    }

    void OnCollisionStay2D(Collision2D col)
    {
        TryCollect(col.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        TryCollect(other.gameObject);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        TryCollect(other.gameObject);
    }

    private void TryCollect(GameObject other)
    {
        if (!readyToCollect || consumed)
            return;

        // 접촉한 대상이 Player인지 확인
        Rigidbody2D otherBody = other.GetComponentInParent<Rigidbody2D>();

        bool isPlayer = other.CompareTag("Player") ||
            (otherBody != null && otherBody.CompareTag("Player"));

        if (!isPlayer)
            return;

        consumed = true;
        readyToCollect = false;

        // 마리오를 직접 호출하지 않고 SO 이벤트 발생
        onPowerupCollected.Raise(this);

        mushroomRenderer.enabled = false;
        mushroomCollider.enabled = false;

        rigidBody.linearVelocity = Vector2.zero;
        rigidBody.angularVelocity = 0f;
        rigidBody.simulated = false;
    }

    public override void SpawnPowerup()
    {
        if (spawned)
            return;

        spawned = true;
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        consumed = false;
        readyToCollect = false;

        mushroomAnimator.enabled = false;
        mushroomRenderer.enabled = true;

        Color color = mushroomRenderer.color;
        color.a = 1f;
        mushroomRenderer.color = color;

        rigidBody.simulated = true;
        rigidBody.bodyType = RigidbodyType2D.Dynamic;
        rigidBody.gravityScale = 1f;

        yield return new WaitForFixedUpdate();

        rigidBody.AddForce(
            Vector2.right * 3f,
            ForceMode2D.Impulse
        );

        readyToCollect = true;
        mushroomCollider.enabled = true;
    }

    public override void ApplyPowerup(MonoBehaviour target)
    {
        if (target is IPowerupApplicable applicable)
        {
            applicable.RequestPowerupEffect(this);
        }
    }

    public void GameRestart()
    {
        StopAllCoroutines();

        PrepareHidden();

        mushroomRenderer.enabled = true;

        mushroomAnimator.enabled = true;
        mushroomAnimator.Rebind();
        mushroomAnimator.Update(0f);

        transform.localPosition = startingPosition;

        Color color = mushroomRenderer.color;
        color.a = 0f;
        mushroomRenderer.color = color;
    }
}