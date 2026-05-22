using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private int maxSlots = 24;

    private readonly List<InventoryItem> items = new();

    public IReadOnlyList<InventoryItem> Items => items;
    public int MaxSlots => maxSlots;

    public event Action OnInventoryChanged;

    public bool AddItem(ItemDefinition itemDefinition, int quantity = 1)
    {
        if (itemDefinition == null || quantity <= 0)
        {
            return false;
        }

        InventoryItem existingItem = items.Find(x => x.ItemDefinition == itemDefinition);

        if (existingItem != null)
        {
            existingItem.AddQuantity(quantity);
            OnInventoryChanged?.Invoke();
            return true;
        }

        if (items.Count >= maxSlots)
        {
            Debug.Log("Inventory is full.");
            return false;
        }

        items.Add(new InventoryItem(itemDefinition, quantity));
        OnInventoryChanged?.Invoke();

        Debug.Log($"Added {itemDefinition.itemName} x{quantity} to inventory.");
        return true;
    }

    public bool RemoveItem(ItemDefinition itemDefinition, int quantity = 1)
    {
        if (itemDefinition == null || quantity <= 0)
        {
            return false;
        }

        InventoryItem existingItem = items.Find(x => x.ItemDefinition == itemDefinition);

        if (existingItem == null)
        {
            return false;
        }

        existingItem.RemoveQuantity(quantity);

        if (existingItem.Quantity <= 0)
        {
            items.Remove(existingItem);
        }

        OnInventoryChanged?.Invoke();
        return true;
    }

    public bool HasSpaceFor(ItemDefinition itemDefinition)
    {
        if (itemDefinition == null)
        {
            return false;
        }

        if (items.Exists(x => x.ItemDefinition == itemDefinition))
        {
            return true;
        }

        return items.Count < maxSlots;
    }
}