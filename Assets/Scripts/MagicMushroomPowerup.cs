using System.Collections;
using UnityEngine;

public class MagicMushroomPowerup : BasePowerup
{
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

        GameManager.instance.gameRestart.AddListener(GameRestart);
    }

    void OnDestroy()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.gameRestart.RemoveListener(GameRestart);
        }
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

        // 横方向の壁に当たったら進行方向を変える
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

        PlayerMovement player =
            other.GetComponentInParent<PlayerMovement>();

        if (player == null || !player.alive)
            return;

        consumed = true;

        ApplyPowerup(player);

        // Destroyせず隠すのでリスタート時に復元できる
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

        // Dynamicへの変更後、物理更新を待つ
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