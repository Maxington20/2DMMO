using UnityEngine;

public class EnemyRangedAttackController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyCombatController meleeCombatController;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;

    [Header("Ranged Attack")]
    [SerializeField] private float attackRange = 4.5f;
    [SerializeField] private float attackSpeedSeconds = 1.5f;
    [SerializeField] private int attackDamage = 3;

    private float attackTimer;

    private void Awake()
    {
        if (meleeCombatController == null)
        {
            meleeCombatController = GetComponent<EnemyCombatController>();
        }
    }

    private void Update()
    {
        attackTimer -= Time.deltaTime;

        CombatEntity target = FindClosestTarget();

        if (target == null)
        {
            return;
        }

        float distance = Vector2.Distance(transform.position, target.transform.position);

        if (distance > attackRange)
        {
            return;
        }

        if (attackTimer <= 0f)
        {
            FireProjectile(target);
            attackTimer = attackSpeedSeconds;
        }
    }

    private void FireProjectile(CombatEntity target)
    {
        if (projectilePrefab == null || target == null)
        {
            return;
        }

        Vector3 spawnPosition = projectileSpawnPoint != null
            ? projectileSpawnPoint.position
            : transform.position;

        GameObject projectileObject = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();

        if (projectile != null)
        {
            projectile.Initialize(target.transform, gameObject, attackDamage);
        }
    }

    private CombatEntity FindClosestTarget()
    {
        CombatEntity[] entities = FindObjectsOfType<CombatEntity>();

        CombatEntity closestTarget = null;
        float closestDistance = float.MaxValue;

        foreach (CombatEntity entity in entities)
        {
            if (entity == null)
            {
                continue;
            }

            if (!entity.CanBeTargetedByEnemies || !entity.IsAlive)
            {
                continue;
            }

            if (entity.gameObject == gameObject)
            {
                continue;
            }

            float distance = Vector2.Distance(transform.position, entity.transform.position);

            if (distance > attackRange)
            {
                continue;
            }

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = entity;
            }
        }

        return closestTarget;
    }
}