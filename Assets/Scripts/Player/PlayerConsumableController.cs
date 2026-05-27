using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerConsumableController : MonoBehaviour
{
    [Header("Potion")]
    [SerializeField] private ItemDefinition healthPotionItem;
    [SerializeField] private float potionCooldownSeconds = 10f;

    [Header("References")]
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private Health playerHealth;

    private float cooldownTimer;

    public bool IsPotionOnCooldown => cooldownTimer > 0f;
    public float PotionCooldownTimer => cooldownTimer;
    public float PotionCooldownSeconds => potionCooldownSeconds;

    private void Awake()
    {
        if (inventory == null)
        {
            inventory = GetComponent<PlayerInventory>();
        }

        if (playerHealth == null)
        {
            playerHealth = GetComponent<Health>();
        }
    }

    private void Update()
    {
        TickCooldown();

        if (GameplayInputLock.IsLocked)
        {
            return;
        }

        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            return;
        }

        if (keyboard.digit2Key.wasPressedThisFrame || keyboard.numpad2Key.wasPressedThisFrame)
        {
            UseAssignedHealthPotion();
        }
    }

    public bool UseConsumable(ItemDefinition consumableItem)
    {
        if (consumableItem == null || consumableItem.itemType != ItemType.Consumable)
        {
            return false;
        }

        if (cooldownTimer > 0f)
        {
            ChatManager.Instance?.AddSystemMessage($"{consumableItem.itemName} is on cooldown.");
            return false;
        }

        if (inventory == null || playerHealth == null)
        {
            return false;
        }

        bool removed = inventory.RemoveItem(consumableItem, 1);

        if (!removed)
        {
            ChatManager.Instance?.AddSystemMessage($"You do not have {consumableItem.itemName}.");
            return false;
        }

        if (consumableItem.restoreHealthAmount > 0)
        {
            playerHealth.Heal(consumableItem.restoreHealthAmount);

            ChatManager.Instance?.AddSystemMessage(
                $"You use {consumableItem.itemName} and restore {consumableItem.restoreHealthAmount} health."
            );
        }
        else
        {
            ChatManager.Instance?.AddSystemMessage($"You use {consumableItem.itemName}.");
        }

        cooldownTimer = potionCooldownSeconds;
        return true;
    }

    private void UseAssignedHealthPotion()
    {
        if (healthPotionItem == null)
        {
            return;
        }

        UseConsumable(healthPotionItem);
    }

    private void TickCooldown()
    {
        if (cooldownTimer <= 0f)
        {
            return;
        }

        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer < 0f)
        {
            cooldownTimer = 0f;
        }
    }
}