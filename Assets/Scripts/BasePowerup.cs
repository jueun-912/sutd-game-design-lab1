using UnityEngine;

public abstract class BasePowerup : MonoBehaviour, IPowerup
{
    public PowerupType type;
    public bool spawned = false;

    protected bool consumed = false;
    protected bool goRight = true;
    protected Rigidbody2D rigidBody;

    protected virtual void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    public PowerupType powerupType
    {
        get { return type; }
    }

    public bool hasSpawned
    {
        get { return spawned; }
    }

    public void DestroyPowerup()
    {
        Destroy(gameObject);
    }

    public abstract void SpawnPowerup();
    public abstract void ApplyPowerup(MonoBehaviour i);
}