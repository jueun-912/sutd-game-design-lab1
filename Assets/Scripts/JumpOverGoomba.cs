using UnityEngine;

public class JumpOverGoomba : MonoBehaviour
{
    public Transform enemyLocation;

    private bool onGroundState;
    private bool countScoreState = false;

    public Vector3 boxSize;
    public float maxDistance;
    public LayerMask layerMask;

    private GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("Manager")
            .GetComponent<GameManager>();
    }
    void FixedUpdate()
    {
        // Mario starts jumping
        if (Input.GetKeyDown("space") && onGroundCheck())
        {
            onGroundState = false;
            countScoreState = true;
        }

        // when jumping, and Goomba is near Mario
        // and we haven't registered the score yet
        if (!onGroundState && countScoreState)
        {
            if (Mathf.Abs(
                transform.position.x - enemyLocation.position.x
            ) < 0.5f)
            {
                countScoreState = false;

                // GameManager now owns the score
                gameManager.IncreaseScore(1);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            onGroundState = true;
        }
    }

    private bool onGroundCheck()
    {
        if (Physics2D.BoxCast(
            transform.position,
            boxSize,
            0,
            -transform.up,
            maxDistance,
            layerMask
        ))
        {
            Debug.Log("on ground");
            return true;
        }
        else
        {
            Debug.Log("not on ground");
            return false;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(
            transform.position - transform.up * maxDistance,
            boxSize
        );
    }
}