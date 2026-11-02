using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public Transform endLimit;

    private float offset;
    private float startX;
    private float endX;
    private float viewportHalfWidth;
    private Vector3 startPosition;

    void Awake()
    {
        startPosition = transform.position;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        Camera cam = GetComponent<Camera>();
        Vector3 bottomLeft = cam.ViewportToWorldPoint(
            new Vector3(0, 0, 0)
        );

        viewportHalfWidth = Mathf.Abs(
            bottomLeft.x - transform.position.x
        );

        offset = transform.position.x - player.position.x;
        startX = startPosition.x;
        endX = endLimit.position.x - viewportHalfWidth;
    }

    void LateUpdate()
    {
        if (player == null) return;

        float desiredX = player.position.x + offset;

        transform.position = new Vector3(
            Mathf.Clamp(desiredX, startX, endX),
            transform.position.y,
            transform.position.z
        );
    }

    public void GameRestart()
    {
        transform.position = startPosition;
    }
}