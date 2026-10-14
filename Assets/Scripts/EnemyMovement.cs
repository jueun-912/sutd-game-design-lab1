using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public GameConstants gameConstants;

    [System.NonSerialized]
    public Vector3 startPosition;

    private float originalX;
    private float maxOffset;
    private float enemyPatroltime;
    private int moveRight = -1;
    private Vector2 velocity;

    private Rigidbody2D enemyBody;

    void Start()
    {
        maxOffset = gameConstants.goombaMaxOffset;
        enemyPatroltime = gameConstants.goombaPatrolTime;

        enemyBody = GetComponent<Rigidbody2D>();

        startPosition = transform.localPosition;
        originalX = transform.position.x;

        ComputeVelocity();
    }

    void ComputeVelocity()
    {
        velocity = new Vector2(
            moveRight * maxOffset / enemyPatroltime,
            0
        );
    }

    void Movegoomba()
    {
        enemyBody.MovePosition(
            enemyBody.position + velocity * Time.fixedDeltaTime
        );
    }

    void FixedUpdate()
    {
        if (Mathf.Abs(enemyBody.position.x - originalX) < maxOffset)
        {
            Movegoomba();
        }
        else
        {
            moveRight *= -1;
            ComputeVelocity();
            Movegoomba();
        }
    }

    public void GameRestart()
    {
        transform.localPosition = startPosition;
        originalX = transform.position.x;
        moveRight = -1;
        ComputeVelocity();
    }
}