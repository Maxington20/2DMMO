using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 30;

    private int currentHealth;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => currentHealth <= 0;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        if (IsDead)
        {
            return;
        }

        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(currentHealth, 0);

        FloatingCombatTextSpawner.Instance?.SpawnDamageText(transform.position, damageAmount);

        if (IsDead)
        {
            Die();
        }
    }

    private void Die()
    {
        gameObject.SetActive(false);
    }
}