using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FakePlayerCombatController : MonoBehaviour
{
    private enum FakePlayerState
    {
        Idle,
        Chasing,
        Attacking
    }

    [Header("Detection")]
    [SerializeField] private float detectionRange = 5f;

    [Header("Combat")]
    [SerializeField] private float attackRange = 1.4f;
    [SerializeField] private float attackSpeedSeconds = 2f;
    [SerializeField] private int damage = 4;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.2f;

    private Rigidbody2D rb;
    private Enemy currentTarget;
    private FakePlayerState currentState;

    private float attackTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        switch (currentState)
        {
            case FakePlayerState.Idle:
                HandleIdle();
                break;

            case FakePlayerState.Chasing:
                HandleChasing();
                break;

            case FakePlayerState.Attacking:
                HandleAttacking();
                break;
        }
    }

    private void HandleIdle()
    {
        FindTarget();
    }

    private void HandleChasing()
    {
        if (!IsTargetValid())
        {
            ClearTarget();
            return;
        }

        float distance =
            Vector2.Distance(transform.position, currentTarget.transform.position);

        if (distance <= attackRange)
        {
            currentState = FakePlayerState.Attacking;
            return;
        }

        Vector2 direction =
            (currentTarget.transform.position - transform.position).normalized;

        Vector2 nextPosition =
            rb.position + direction * moveSpeed * Time.deltaTime;

        rb.MovePosition(nextPosition);
    }

    private void HandleAttacking()
    {
        if (!IsTargetValid())
        {
            ClearTarget();
            return;
        }

        float distance =
            Vector2.Distance(transform.position, currentTarget.transform.position);

        if (distance > attackRange)
        {
            currentState = FakePlayerState.Chasing;
            return;
        }

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            PerformAttack();
            attackTimer = attackSpeedSeconds;
        }
    }

    private void PerformAttack()
    {
        if (!IsTargetValid())
        {
            return;
        }

        Health targetHealth = currentTarget.GetComponent<Health>();

        if (targetHealth == null)
        {
            return;
        }

        targetHealth.TakeDamage(damage);

        Debug.Log($"{gameObject.name} attacks {currentTarget.EnemyName} for {damage}.");
    }

    private void FindTarget()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();

        Enemy closestEnemy = null;
        float closestDistance = float.MaxValue;

        foreach (Enemy enemy in enemies)
        {
            if (enemy == null)
            {
                continue;
            }

            Health health = enemy.GetComponent<Health>();

            if (health == null || health.IsDead)
            {
                continue;
            }

            float distance =
                Vector2.Distance(transform.position, enemy.transform.position);

            if (distance > detectionRange)
            {
                continue;
            }

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        if (closestEnemy != null)
        {
            currentTarget = closestEnemy;
            currentState = FakePlayerState.Chasing;
        }
    }

    private bool IsTargetValid()
    {
        if (currentTarget == null)
        {
            return false;
        }

        Health health = currentTarget.GetComponent<Health>();

        if (health == null || health.IsDead)
        {
            return false;
        }

        return true;
    }

    private void ClearTarget()
    {
        currentTarget = null;
        currentState = FakePlayerState.Idle;
    }
}