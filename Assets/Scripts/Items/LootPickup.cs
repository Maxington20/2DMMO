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

        Debug.Log($"Picked up {item.itemName} [{item.rarity}] worth {item.sellValue} copper.");
        Destroy(gameObject);
    }
}