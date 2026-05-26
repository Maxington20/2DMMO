using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAbilityController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerTargetingController targetingController;
    [SerializeField] private PlayerEquipment playerEquipment;

    [Header("Primary Ability")]
    [SerializeField] private string primaryAbilityName = "Strike";
    [SerializeField] private float primaryAbilityRange = 1.6f;
    [SerializeField] private int primaryAbilityBaseDamage = 10;
    [SerializeField] private float primaryAbilityCooldownSeconds = 3f;

    private float primaryAbilityCooldownTimer;

    public string PrimaryAbilityName => primaryAbilityName;

    private void Awake()
    {
        if (targetingController == null)
        {
            targetingController = GetComponent<PlayerTargetingController>();
        }

        if (playerEquipment == null)
        {
            playerEquipment = GetComponent<PlayerEquipment>();
        }
    }

    private void Update()
    {
        TickCooldowns();

        if (GameplayInputLock.IsLocked)
        {
            return;
        }

        HandleInput();
    }

    public void ConfigurePrimaryAbilityForClass(string className)
    {
        switch (className)
        {
            case "Wizard":
                primaryAbilityName = "Firebolt";
                primaryAbilityRange = 5f;
                primaryAbilityBaseDamage = 12;
                primaryAbilityCooldownSeconds = 2.5f;
                break;

            case "Priest":
                primaryAbilityName = "Smite";
                primaryAbilityRange = 4.5f;
                primaryAbilityBaseDamage = 9;
                primaryAbilityCooldownSeconds = 2.5f;
                break;

            case "Ranger":
                primaryAbilityName = "Shot";
                primaryAbilityRange = 5f;
                primaryAbilityBaseDamage = 10;
                primaryAbilityCooldownSeconds = 2.2f;
                break;

            case "Rogue":
                primaryAbilityName = "Stab";
                primaryAbilityRange = 1.3f;
                primaryAbilityBaseDamage = 13;
                primaryAbilityCooldownSeconds = 2f;
                break;

            case "Necromancer":
                primaryAbilityName = "Shadow Bolt";
                primaryAbilityRange = 4.5f;
                primaryAbilityBaseDamage = 11;
                primaryAbilityCooldownSeconds = 2.8f;
                break;

            case "Artificer":
                primaryAbilityName = "Spark Shot";
                primaryAbilityRange = 4.5f;
                primaryAbilityBaseDamage = 10;
                primaryAbilityCooldownSeconds = 2.3f;
                break;

            case "Warrior":
            default:
                primaryAbilityName = "Strike";
                primaryAbilityRange = 1.6f;
                primaryAbilityBaseDamage = 10;
                primaryAbilityCooldownSeconds = 3f;
                break;
        }

        ChatManager.Instance?.AddSystemMessage($"Primary ability set to {primaryAbilityName}.");
    }

    private void TickCooldowns()
    {
        if (primaryAbilityCooldownTimer > 0f)
        {
            primaryAbilityCooldownTimer -= Time.deltaTime;
        }
    }

    private void HandleInput()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            return;
        }

        if (keyboard.digit1Key.wasPressedThisFrame || keyboard.numpad1Key.wasPressedThisFrame)
        {
            TryUsePrimaryAbility();
        }
    }

    private void TryUsePrimaryAbility()
    {
        if (primaryAbilityCooldownTimer > 0f)
        {
            Debug.Log($"{primaryAbilityName} is on cooldown: {primaryAbilityCooldownTimer:0.0}s remaining.");
            return;
        }

        if (targetingController == null || targetingController.CurrentTarget == null)
        {
            Debug.Log("No target selected.");
            return;
        }

        Enemy target = targetingController.CurrentTarget;

        if (!target.gameObject.activeInHierarchy)
        {
            Debug.Log("Target is no longer active.");
            return;
        }

        float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);

        if (distanceToTarget > primaryAbilityRange)
        {
            Debug.Log("Target is out of range.");
            return;
        }

        Health targetHealth = target.GetComponent<Health>();

        if (targetHealth == null || targetHealth.IsDead)
        {
            Debug.Log("Target has no health or is already dead.");
            return;
        }

        int totalDamage = primaryAbilityBaseDamage + GetBonusDamage();

        targetHealth.TakeDamage(totalDamage, gameObject);
        primaryAbilityCooldownTimer = primaryAbilityCooldownSeconds;

        Debug.Log($"Used {primaryAbilityName} on {target.EnemyName} for {totalDamage} damage.");
    }

    private int GetBonusDamage()
    {
        return playerEquipment != null ? playerEquipment.BonusDamage : 0;
    }
}