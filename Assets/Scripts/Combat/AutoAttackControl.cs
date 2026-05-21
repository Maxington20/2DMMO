using UnityEngine;

public class AutoAttackController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerTargetingController targetingController;

    [Header("Auto Attack")]
    [SerializeField] private float attackRange = 1.4f;
    [SerializeField] private float attackSpeedSeconds = 2f;
    [SerializeField] private int damage = 5;

    private float attackTimer;

    private void Awake()
    {
        if (targetingController == null)
        {
            targetingController = GetComponent<PlayerTargetingController>();
        }
    }

    private void Update()
    {
        if (targetingController == null)
        {
            return;
        }

        Enemy target = targetingController.CurrentTarget;

        if (target == null || !target.gameObject.activeInHierarchy)
        {
            attackTimer = 0f;
            return;
        }

        Health targetHealth = target.GetComponent<Health>();

        if (targetHealth == null || targetHealth.IsDead)
        {
            attackTimer = 0f;
            return;
        }

        float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);

        if (distanceToTarget > attackRange)
        {
            attackTimer = 0f;
            return;
        }

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            PerformAttack(targetHealth);
            attackTimer = attackSpeedSeconds;
        }
    }

    private void PerformAttack(Health targetHealth)
    {
        Debug.Log($"{gameObject.name} auto-attacks {targetHealth.gameObject.name} for {damage} damage.");
        targetHealth.TakeDamage(damage);
    }
}