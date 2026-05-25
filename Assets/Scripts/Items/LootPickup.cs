using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class LootPickup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private ItemDefinition item;

    public void Initialize(ItemDefinition itemDefinition)
    {
        item = itemDefinition;

        if (spriteRenderer != null && item != null && item.icon != null)
        {
            spriteRenderer.sprite = item.icon;
        }

        gameObject.name = item != null ? $"Loot_{item.itemName}" : "Loot_Unknown";
    }

    private void Update()
    {
        if (GameplayInputLock.ShouldBlockWorldClick())
        {
            return;
        }

        Mouse mouse = Mouse.current;

        if (mouse == null || !mouse.leftButton.wasPressedThisFrame)
        {
            return;
        }

        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            return;
        }

        Vector2 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouse.position.ReadValue());
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero);

        if (hit.collider == null || hit.collider.gameObject != gameObject)
        {
            return;
        }

        Pickup();
    }

    private void Pickup()
    {
        if (item == null)
        {
            Destroy(gameObject);
            return;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            Debug.LogWarning("Could not find Player object.");
            return;
        }

        PlayerInventory inventory = playerObject.GetComponent<PlayerInventory>();

        if (inventory == null)
        {
            Debug.LogWarning("Player does not have PlayerInventory.");
            return;
        }

        bool added = inventory.AddItem(item, 1);

        if (added)
        {
            ChatManager.Instance?.AddSystemMessage($"You looted {item.itemName}.");
            Destroy(gameObject);
        }
    }
}