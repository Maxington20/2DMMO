using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Enemy))]
[RequireComponent(typeof(Health))]
public class EnemyCombatController : MonoBehaviour
{
    private enum EnemyState
    {
        Idle,
        Chasing,
        Attacking,
        ReturningHome
    }

    [Header("Target")]
    [SerializeField] private Transform playerTarget;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.2f;
    [SerializeField] private float aggroRange = 4f;
    [SerializeField] private float attackRange = 1.1f;
    [SerializeField] private float leashRange = 7f;
    [SerializeField] private float homeArrivalDistance = 0.05f;

    [Header("Attack")]
    [SerializeField] private int attackDamage = 4;
    [SerializeField] private float attackSpeedSeconds = 2f;

    private Rigidbody2D rb;
    private Health playerHealth;
    private Vector2 homePosition;
    private EnemyState currentState = EnemyState.Idle;
    private float attackTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        homePosition = rb.position;
    }

    private void Start()
    {
        if (playerTarget == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                playerTarget = playerObject.transform;
            }
        }

        if (playerTarget != null)
        {
            playerHealth = playerTarget.GetComponent<Health>();
        }
    }

    private void FixedUpdate()
    {
        if (playerTarget == null || playerHealth == null || playerHealth.IsDead)
        {
            ReturnHomeOrIdle();
            return;
        }

        float distanceToPlayer = Vector2.Distance(rb.position, playerTarget.position);
        float distanceFromHome = Vector2.Distance(rb.position, homePosition);

        switch (currentState)
        {
            case EnemyState.Idle:
                HandleIdle(distanceToPlayer);
                break;

            case EnemyState.Chasing:
                HandleChasing(distanceToPlayer, distanceFromHome);
                break;

            case EnemyState.Attacking:
                HandleAttacking(distanceToPlayer, distanceFromHome);
                break;

            case EnemyState.ReturningHome:
                HandleReturningHome();
                break;
        }
    }

    private void HandleIdle(float distanceToPlayer)
    {
        if (distanceToPlayer <= aggroRange)
        {
            currentState = EnemyState.Chasing;
        }
    }

    private void HandleChasing(float distanceToPlayer, float distanceFromHome)
    {
        if (distanceFromHome > leashRange)
        {
            currentState = EnemyState.ReturningHome;
            return;
        }

        if (distanceToPlayer <= attackRange)
        {
            currentState = EnemyState.Attacking;
            return;
        }

        MoveToward(playerTarget.position);
    }

    private void HandleAttacking(float distanceToPlayer, float distanceFromHome)
    {
        if (distanceFromHome > leashRange)
        {
            currentState = EnemyState.ReturningHome;
            return;
        }

        if (distanceToPlayer > attackRange)
        {
            currentState = EnemyState.Chasing;
            return;
        }

        attackTimer -= Time.fixedDeltaTime;

        if (attackTimer <= 0f)
        {
            AttackPlayer();
            attackTimer = attackSpeedSeconds;
        }
    }

    private void HandleReturningHome()
    {
        float distanceToHome = Vector2.Distance(rb.position, homePosition);

        if (distanceToHome <= homeArrivalDistance)
        {
            rb.MovePosition(homePosition);
            currentState = EnemyState.Idle;
            attackTimer = 0f;
            return;
        }

        MoveToward(homePosition);
    }

    private void ReturnHomeOrIdle()
    {
        float distanceToHome = Vector2.Distance(rb.position, homePosition);

        if (distanceToHome > homeArrivalDistance)
        {
            currentState = EnemyState.ReturningHome;
        }
        else
        {
            currentState = EnemyState.Idle;
        }
    }

    private void MoveToward(Vector2 destination)
    {
        Vector2 currentPosition = rb.position;
        Vector2 direction = destination - currentPosition;

        if (direction.sqrMagnitude <= 0.001f)
        {
            return;
        }

        Vector2 nextPosition = currentPosition + direction.normalized * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(nextPosition);
    }

    private void AttackPlayer()
    {
        if (playerHealth == null || playerHealth.IsDead)
        {
            return;
        }

        playerHealth.TakeDamage(attackDamage);
        Debug.Log($"{gameObject.name} attacks player for {attackDamage} damage.");
    }
}