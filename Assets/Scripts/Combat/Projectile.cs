using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float lifetimeSeconds = 3f;
    [SerializeField] private float hitDistance = 0.15f;

    private Transform target;
    private GameObject attacker;
    private int damage;
    private bool hasHit;

    public void Initialize(Transform targetTransform, GameObject attackerObject, int damageAmount)
    {
        target = targetTransform;
        attacker = attackerObject;
        damage = damageAmount;

        IgnoreAttackerColliders();

        Destroy(gameObject, lifetimeSeconds);
    }

    private void Update()
    {
        if (hasHit)
        {
            return;
        }

        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = (target.position - transform.position).normalized;

        transform.position += direction * moveSpeed * Time.deltaTime;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        if (distanceToTarget <= hitDistance)
        {
            TryHitTargetDirectly();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit)
        {
            return;
        }

        if (IsColliderPartOfAttacker(other))
        {
            return;
        }

        Health targetHealth = other.GetComponentInParent<Health>();

        if (targetHealth == null || targetHealth.IsDead)
        {
            return;
        }

        Hit(targetHealth);
    }

    private void TryHitTargetDirectly()
    {
        if (target == null)
        {
            return;
        }

        Health targetHealth = target.GetComponentInParent<Health>();

        if (targetHealth == null || targetHealth.IsDead)
        {
            return;
        }

        Hit(targetHealth);
    }

    private void Hit(Health targetHealth)
    {
        if (hasHit || targetHealth == null)
        {
            return;
        }

        hasHit = true;
        targetHealth.TakeDamage(damage, attacker);
        Destroy(gameObject);
    }

    private void IgnoreAttackerColliders()
    {
        if (attacker == null)
        {
            return;
        }

        Collider2D projectileCollider = GetComponent<Collider2D>();

        if (projectileCollider == null)
        {
            return;
        }

        Collider2D[] attackerColliders = attacker.GetComponentsInChildren<Collider2D>();

        foreach (Collider2D attackerCollider in attackerColliders)
        {
            if (attackerCollider != null)
            {
                Physics2D.IgnoreCollision(projectileCollider, attackerCollider, true);
            }
        }
    }

    private bool IsColliderPartOfAttacker(Collider2D other)
    {
        if (attacker == null || other == null)
        {
            return false;
        }

        return other.transform == attacker.transform ||
               other.transform.IsChildOf(attacker.transform);
    }
}