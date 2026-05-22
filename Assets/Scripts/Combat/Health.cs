using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 30;

    [Header("Respawn")]
    [SerializeField] private bool canRespawn = false;
    [SerializeField] private float respawnDelay = 5f;

    private int currentHealth;
    private bool isDead;
    private Vector3 spawnPosition;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
        spawnPosition = transform.position;
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(currentHealth, 0);

        FloatingCombatTextSpawner.Instance?.SpawnDamageText(transform.position, damageAmount);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void RestoreFullHealth()
    {
        currentHealth = maxHealth;
        isDead = false;
    }

    private void Die()
    {
        isDead = true;

        Debug.Log($"{gameObject.name} died.");

        if (CompareTag("Player"))
        {
            StartCoroutine(PlayerRespawnRoutine());
        }
        else if (canRespawn)
        {
            StartCoroutine(EnemyRespawnRoutine());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator PlayerRespawnRoutine()
    {
        HandlePlayerDeathState(true);

        yield return new WaitForSeconds(respawnDelay);

        transform.position = spawnPosition;

        RestoreFullHealth();

        HandlePlayerDeathState(false);
    }

    private IEnumerator EnemyRespawnRoutine()
    {
        Transform visual = transform.Find("WolfVisual");

        if (visual != null)
        {
            visual.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(respawnDelay);

        transform.position = spawnPosition;

        RestoreFullHealth();

        if (visual != null)
        {
            visual.gameObject.SetActive(true);
        }
    }

    private void HandlePlayerDeathState(bool dead)
    {
        PlayerMovementController movement =
            GetComponent<PlayerMovementController>();

        if (movement != null)
        {
            movement.enabled = !dead;
        }

        PlayerTargetingController targeting =
            GetComponent<PlayerTargetingController>();

        if (targeting != null)
        {
            targeting.enabled = !dead;
        }

        AutoAttackController autoAttack =
            GetComponent<AutoAttackController>();

        if (autoAttack != null)
        {
            autoAttack.enabled = !dead;
        }

        PlayerAbilityController abilities =
            GetComponent<PlayerAbilityController>();

        if (abilities != null)
        {
            abilities.enabled = !dead;
        }
    }
}