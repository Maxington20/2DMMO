using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 30;

    [Header("Respawn")]
    [SerializeField] private bool canRespawn = false;
    [SerializeField] private float respawnDelay = 5f;
    [SerializeField] private GameObject visualRoot;

    [Header("Death Reward")]
    [SerializeField] private int xpReward = 0;

    private int currentHealth;
    private bool isDead;
    private Vector3 spawnPosition;
    private GameObject lastAttacker;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => isDead;
    public int XpReward => xpReward;

    private void Awake()
    {
        spawnPosition = transform.position;
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        TakeDamage(damageAmount, null);
    }

    public void TakeDamage(int damageAmount, GameObject attacker)
    {
        if (isDead)
        {
            return;
        }

        lastAttacker = attacker;

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
        lastAttacker = null;
    }

    public void IncreaseMaxHealth(int amount, bool restoreToFull)
    {
        if (amount <= 0)
        {
            return;
        }

        maxHealth += amount;

        if (restoreToFull)
        {
            RestoreFullHealth();
        }
        else
        {
            currentHealth = Mathf.Min(currentHealth, maxHealth);
        }
    }

    private void Die()
    {
        isDead = true;

        Debug.Log($"{gameObject.name} died.");

        if (CompareTag("Player"))
        {
            ChatManager.Instance?.AddSystemMessage("You died.");
            StartCoroutine(PlayerRespawnRoutine());
            return;
        }

        AwardXpToKiller();
        RegisterQuestKillForKiller();

        LootDropper lootDropper = GetComponent<LootDropper>();

        if (lootDropper != null)
        {
            lootDropper.DropLoot();
        }

        if (canRespawn)
        {
            StartCoroutine(GenericRespawnRoutine());
        }
        else
        {
            SetVisibleAndCollidable(false);
        }
    }

    private void AwardXpToKiller()
    {
        if (xpReward <= 0 || lastAttacker == null)
        {
            return;
        }

        PlayerProgression progression = lastAttacker.GetComponent<PlayerProgression>();

        if (progression != null)
        {
            progression.AddXp(xpReward);
        }
    }

    private void RegisterQuestKillForKiller()
    {
        if (lastAttacker == null)
        {
            return;
        }

        PlayerQuestLog questLog = lastAttacker.GetComponent<PlayerQuestLog>();

        if (questLog == null)
        {
            return;
        }

        Enemy enemy = GetComponent<Enemy>();

        if (enemy != null)
        {
            questLog.RegisterEnemyKill(enemy);
        }
    }

    private IEnumerator PlayerRespawnRoutine()
    {
        HandlePlayerDeathState(true);

        yield return new WaitForSeconds(respawnDelay);

        transform.position = spawnPosition;
        RestoreFullHealth();

        HandlePlayerDeathState(false);

        ChatManager.Instance?.AddSystemMessage("You have respawned.");
    }

    private IEnumerator GenericRespawnRoutine()
    {
        SetVisibleAndCollidable(false);
        SetBehaviourEnabled(false);

        yield return new WaitForSeconds(respawnDelay);

        transform.position = spawnPosition;
        RestoreFullHealth();

        SetVisibleAndCollidable(true);
        SetBehaviourEnabled(true);
    }

    private void SetVisibleAndCollidable(bool enabled)
    {
        if (visualRoot != null)
        {
            visualRoot.SetActive(enabled);
            return;
        }

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = enabled;
        }

        Collider2D collider = GetComponent<Collider2D>();

        if (collider != null)
        {
            collider.enabled = enabled;
        }

        foreach (SpriteRenderer childRenderer in GetComponentsInChildren<SpriteRenderer>())
        {
            childRenderer.enabled = enabled;
        }

        foreach (Collider2D childCollider in GetComponentsInChildren<Collider2D>())
        {
            childCollider.enabled = enabled;
        }
    }

    private void SetBehaviourEnabled(bool enabled)
    {
        EnemyCombatController enemyCombat = GetComponent<EnemyCombatController>();

        if (enemyCombat != null)
        {
            enemyCombat.enabled = enabled;
        }

        FakePlayerCombatController fakeCombat = GetComponent<FakePlayerCombatController>();

        if (fakeCombat != null)
        {
            fakeCombat.enabled = enabled;
        }

        FakePlayerController fakeMovement = GetComponent<FakePlayerController>();

        if (fakeMovement != null)
        {
            fakeMovement.enabled = enabled;
        }
    }

    private void HandlePlayerDeathState(bool dead)
    {
        PlayerMovementController movement = GetComponent<PlayerMovementController>();

        if (movement != null)
        {
            movement.enabled = !dead;
        }

        PlayerTargetingController targeting = GetComponent<PlayerTargetingController>();

        if (targeting != null)
        {
            targeting.enabled = !dead;
        }

        AutoAttackController autoAttack = GetComponent<AutoAttackController>();

        if (autoAttack != null)
        {
            autoAttack.enabled = !dead;
        }

        PlayerAbilityController abilities = GetComponent<PlayerAbilityController>();

        if (abilities != null)
        {
            abilities.enabled = !dead;
        }
    }
}