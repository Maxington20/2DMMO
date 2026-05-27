using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerConsumableController : MonoBehaviour
{
    [Header("Potion")]
    [SerializeField] private ItemDefinition healthPotionItem;
    [SerializeField] private int healAmount = 35;
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

        if (keyboard.digit2Key.wasPressedThisFrame ||
            keyboard.numpad2Key.wasPressedThisFrame)
        {
            UsePotion();
        }
    }

    private void TickCooldown()
    {
        if (cooldownTimer <= 0)
        {
            return;
        }

        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer < 0)
        {
            cooldownTimer = 0;
        }
    }

    private void UsePotion()
    {
        if (cooldownTimer > 0)
        {
            return;
        }

        if (inventory == null ||
            playerHealth == null ||
            healthPotionItem == null)
        {
            return;
        }

        bool removed = inventory.RemoveItem(
            healthPotionItem,
            1
        );

        if (!removed)
        {
            ChatManager.Instance?.AddSystemMessage(
                "You have no health potions."
            );

            return;
        }

        playerHealth.Heal(healAmount);

        cooldownTimer = potionCooldownSeconds;

        ChatManager.Instance?.AddSystemMessage(
            $"You drink a {healthPotionItem.itemName} and heal {healAmount} health."
        );
    }
}