using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FakePlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float wanderRadius = 3f;
    [SerializeField] private float minIdleTime = 1f;
    [SerializeField] private float maxIdleTime = 3f;

    private Rigidbody2D rb;
    private Vector2 homePosition;
    private Vector2 targetPosition;
    private bool isMoving;
    private float idleTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        homePosition = rb.position;
    }

    private void Start()
    {
        PickNewDestination();
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            MoveTowardTarget();
        }
        else
        {
            HandleIdle();
        }
    }

    private void MoveTowardTarget()
    {
        Vector2 currentPosition = rb.position;
        Vector2 direction = targetPosition - currentPosition;

        if (direction.magnitude <= 0.05f)
        {
            rb.MovePosition(targetPosition);
            StartIdle();
            return;
        }

        Vector2 nextPosition = currentPosition + direction.normalized * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(nextPosition);
    }

    private void HandleIdle()
    {
        idleTimer -= Time.fixedDeltaTime;

        if (idleTimer <= 0f)
        {
            PickNewDestination();
        }
    }

    private void PickNewDestination()
    {
        Vector2 randomOffset = Random.insideUnitCircle * wanderRadius;
        targetPosition = homePosition + randomOffset;
        isMoving = true;
    }

    private void StartIdle()
    {
        isMoving = false;
        idleTimer = Random.Range(minIdleTime, maxIdleTime);
    }
}