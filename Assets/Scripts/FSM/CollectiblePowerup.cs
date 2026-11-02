using UnityEngine;

public enum CollectibleKind
{
    MagicMushroom = 1,
    StarMan = 3,
    FireFlower = 4
}

public class CollectiblePowerup : BasePowerup
{
    public CollectibleKind kind;
    public PowerupGameEvent onPowerupCollected;

    private SpriteRenderer visual;
    private Collider2D hitbox;
    private Vector3 startPosition;

    private void Awake()
    {
        visual = GetComponent<SpriteRenderer>();
        hitbox = GetComponent<Collider2D>();

        startPosition = transform.position;
    }

    protected override void Start()
    {
        base.Start();

        type = (PowerupType)(int)kind;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (consumed)
            return;

        bool isPlayer = other.CompareTag("Player");

        if (!isPlayer && other.attachedRigidbody != null)
        {
            isPlayer =
                other.attachedRigidbody.CompareTag("Player");
        }

        if (!isPlayer)
            return;

        PlayerMovement player =
            other.GetComponentInParent<PlayerMovement>();

        if (player == null || !player.alive)
            return;

        consumed = true;

        onPowerupCollected.Raise(this);

        visual.enabled = false;
        hitbox.enabled = false;
    }

    public override void SpawnPowerup()
    {
        spawned = true;
        visual.enabled = true;
        hitbox.enabled = true;
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
        consumed = false;
        spawned = false;

        transform.position = startPosition;

        visual.enabled = true;
        hitbox.enabled = true;
    }
}