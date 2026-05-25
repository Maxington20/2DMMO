using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAbilityController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerTargetingController targetingController;
    [SerializeField] private PlayerEquipment playerEquipment;

    [Header("Strike")]
    [SerializeField] private float strikeRange = 1.6f;
    [SerializeField] private int strikeBaseDamage = 10;
    [SerializeField] private float strikeCooldownSeconds = 3f;

    private float strikeCooldownTimer;

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

    private void TickCooldowns()
    {
        if (strikeCooldownTimer > 0f)
        {
            strikeCooldownTimer -= Time.deltaTime;
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
            TryUseStrike();
        }
    }

    private void TryUseStrike()
    {
        if (strikeCooldownTimer > 0f)
        {
            Debug.Log($"Strike is on cooldown: {strikeCooldownTimer:0.0}s remaining.");
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

        if (distanceToTarget > strikeRange)
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

        int totalDamage = strikeBaseDamage + GetBonusDamage();

        targetHealth.TakeDamage(totalDamage, gameObject);
        strikeCooldownTimer = strikeCooldownSeconds;

        Debug.Log($"Used Strike on {target.EnemyName} for {totalDamage} damage.");
    }

    private int GetBonusDamage()
    {
        return playerEquipment != null ? playerEquipment.BonusDamage : 0;
    }
}