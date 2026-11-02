using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Actions/FireAttack")]
public class FireAttackAction : Action
{
    public GameObject attackPrefab;

    public float speed = 8f;
    public float spawnOffset = 0.7f;
    public int maxPrefabInScene = 3;

    public override void Act(StateController controller)
    {
        if (attackPrefab == null)
            return;

        FireballController[] existing =
            Object.FindObjectsByType<FireballController>(
                FindObjectsSortMode.None
            );

        if (existing.Length >= maxPrefabInScene)
            return;

        SpriteRenderer sprite =
            controller.GetComponent<SpriteRenderer>();

        float direction = sprite.flipX ? -1f : 1f;

        Vector3 position =
            controller.transform.position +
            Vector3.right * direction * spawnOffset;

        GameObject fireball = Instantiate(
            attackPrefab,
            position,
            Quaternion.identity
        );

        Rigidbody2D body =
            fireball.GetComponent<Rigidbody2D>();

        body.linearVelocity =
            new Vector2(direction * speed, 0f);
    }
}