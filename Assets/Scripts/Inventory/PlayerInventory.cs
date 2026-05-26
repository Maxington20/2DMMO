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

    public bool AddItem(ItemDefinition itemDefinition, int quantity = 1, bool saveAfterChange = true)
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

            if (saveAfterChange)
            {
                SaveInventory();
            }

            return true;
        }

        if (items.Count >= maxSlots)
        {
            ChatManager.Instance?.AddSystemMessage("Your inventory is full.");
            return false;
        }

        items.Add(new InventoryItem(itemDefinition, quantity));
        OnInventoryChanged?.Invoke();

        if (saveAfterChange)
        {
            SaveInventory();
        }

        return true;
    }

    public bool RemoveItem(ItemDefinition itemDefinition, int quantity = 1, bool saveAfterChange = true)
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

        if (saveAfterChange)
        {
            SaveInventory();
        }

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

    public void LoadInventoryFromSave()
    {
        items.Clear();

        CharacterData characterData = CharacterSaveManager.LoadCharacter();

        if (characterData == null || characterData.InventoryItems == null)
        {
            OnInventoryChanged?.Invoke();
            return;
        }

        foreach (SavedInventoryItem savedItem in characterData.InventoryItems)
        {
            if (savedItem == null || savedItem.Quantity <= 0)
            {
                continue;
            }

            ItemDefinition item = ItemDatabase.Instance?.GetItemById(savedItem.ItemId);

            if (item == null)
            {
                Debug.LogWarning($"Could not load inventory item with ID: {savedItem.ItemId}");
                continue;
            }

            AddItem(item, savedItem.Quantity, false);
        }

        OnInventoryChanged?.Invoke();
    }

    public void SaveInventory()
    {
        CharacterData characterData = CharacterSaveManager.LoadCharacter();

        if (characterData == null)
        {
            return;
        }

        characterData.InventoryItems.Clear();

        foreach (InventoryItem inventoryItem in items)
        {
            if (inventoryItem == null || inventoryItem.ItemDefinition == null)
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(inventoryItem.ItemDefinition.itemId))
            {
                Debug.LogWarning($"{inventoryItem.ItemDefinition.itemName} has no itemId and cannot be saved.");
                continue;
            }

            characterData.InventoryItems.Add(
                new SavedInventoryItem(
                    inventoryItem.ItemDefinition.itemId,
                    inventoryItem.Quantity
                )
            );
        }

        CharacterSaveManager.SaveCharacter(characterData);
    }
}