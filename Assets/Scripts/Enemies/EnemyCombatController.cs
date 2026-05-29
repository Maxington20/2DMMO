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

    private enum AttackStyle
    {
        Direct,
        Projectile
    }

    [Header("Targeting")]
    [SerializeField] private float aggroRange = 4f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.2f;
    [SerializeField] private float attackRange = 1.1f;
    [SerializeField] private float leashRange = 7f;
    [SerializeField] private float homeArrivalDistance = 0.05f;

    [Header("Attack")]
    [SerializeField] private AttackStyle attackStyle = AttackStyle.Direct;
    [SerializeField] private int attackDamage = 4;
    [SerializeField] private float attackSpeedSeconds = 2f;

    [Header("Projectile Attack")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;

    private Rigidbody2D rb;
    private Vector2 homePosition;
    private EnemyState currentState = EnemyState.Idle;
    private CombatEntity currentTarget;
    private float attackTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        homePosition = rb.position;
    }

    private void FixedUpdate()
    {
        if (currentTarget != null && !IsTargetValid(currentTarget))
        {
            currentTarget = null;
            currentState = EnemyState.ReturningHome;
        }

        float distanceFromHome = Vector2.Distance(rb.position, homePosition);

        switch (currentState)
        {
            case EnemyState.Idle:
                HandleIdle();
                break;

            case EnemyState.Chasing:
                HandleChasing(distanceFromHome);
                break;

            case EnemyState.Attacking:
                HandleAttacking(distanceFromHome);
                break;

            case EnemyState.ReturningHome:
                HandleReturningHome();
                break;
        }
    }

    private void HandleIdle()
    {
        CombatEntity nearestTarget = FindNearestTarget();

        if (nearestTarget != null)
        {
            currentTarget = nearestTarget;
            currentState = EnemyState.Chasing;
        }
    }

    private void HandleChasing(float distanceFromHome)
    {
        if (currentTarget == null)
        {
            currentState = EnemyState.ReturningHome;
            return;
        }

        if (distanceFromHome > leashRange)
        {
            currentTarget = null;
            currentState = EnemyState.ReturningHome;
            return;
        }

        float distanceToTarget = Vector2.Distance(rb.position, currentTarget.transform.position);

        if (distanceToTarget <= attackRange)
        {
            currentState = EnemyState.Attacking;
            return;
        }

        MoveToward(currentTarget.transform.position);
    }

    private void HandleAttacking(float distanceFromHome)
    {
        if (currentTarget == null)
        {
            currentState = EnemyState.ReturningHome;
            return;
        }

        if (distanceFromHome > leashRange)
        {
            currentTarget = null;
            currentState = EnemyState.ReturningHome;
            return;
        }

        float distanceToTarget = Vector2.Distance(rb.position, currentTarget.transform.position);

        if (distanceToTarget > attackRange)
        {
            currentState = EnemyState.Chasing;
            return;
        }

        attackTimer -= Time.fixedDeltaTime;

        if (attackTimer <= 0f)
        {
            AttackCurrentTarget();
            attackTimer = attackSpeedSeconds;
        }
    }

    private void HandleReturningHome()
    {
        float distanceToHome = Vector2.Distance(rb.position, homePosition);

        if (distanceToHome <= homeArrivalDistance)
        {
            rb.MovePosition(homePosition);
            currentTarget = null;
            currentState = EnemyState.Idle;
            attackTimer = 0f;
            return;
        }

        MoveToward(homePosition);
    }

    private CombatEntity FindNearestTarget()
    {
        CombatEntity[] combatEntities = FindObjectsOfType<CombatEntity>();

        CombatEntity nearestTarget = null;
        float nearestDistance = float.MaxValue;

        foreach (CombatEntity entity in combatEntities)
        {
            if (!IsTargetValid(entity))
            {
                continue;
            }

            float distance = Vector2.Distance(rb.position, entity.transform.position);

            if (distance > aggroRange)
            {
                continue;
            }

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = entity;
            }
        }

        return nearestTarget;
    }

    private bool IsTargetValid(CombatEntity entity)
    {
        if (entity == null)
        {
            return false;
        }

        if (!entity.CanBeTargetedByEnemies)
        {
            return false;
        }

        if (!entity.IsAlive)
        {
            return false;
        }

        return true;
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

    private void AttackCurrentTarget()
    {
        if (currentTarget == null || currentTarget.Health == null)
        {
            return;
        }

        if (attackStyle == AttackStyle.Projectile)
        {
            FireProjectile();
            return;
        }

        currentTarget.Health.TakeDamage(attackDamage);

        Debug.Log($"{gameObject.name} attacks {currentTarget.DisplayName} for {attackDamage} damage.");
    }

    private void FireProjectile()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning($"{gameObject.name} is set to Projectile attack but has no projectile prefab.");
            return;
        }

        Vector3 spawnPosition = projectileSpawnPoint != null
            ? projectileSpawnPoint.position
            : transform.position;

        GameObject projectileObject = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();

        if (projectile == null)
        {
            Debug.LogWarning($"{projectilePrefab.name} does not have a Projectile component.");
            return;
        }

        projectile.Initialize(currentTarget.transform, gameObject, attackDamage);

        Debug.Log($"{gameObject.name} fires a projectile at {currentTarget.DisplayName}.");
    }
}