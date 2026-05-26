using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    private readonly Dictionary<EquipmentSlotType, ItemDefinition> equippedItems = new();

    private PlayerInventory inventory;

    public event Action OnEquipmentChanged;

    public int BonusDamage { get; private set; }
    public int BonusMaxHealth { get; private set; }

    private void Awake()
    {
        inventory = GetComponent<PlayerInventory>();
    }

    public ItemDefinition GetEquippedItem(EquipmentSlotType slotType)
    {
        equippedItems.TryGetValue(slotType, out ItemDefinition item);
        return item;
    }

    public bool Equip(ItemDefinition item)
    {
        if (item == null)
        {
            return false;
        }

        if (item.itemType != ItemType.Equipment)
        {
            ChatManager.Instance?.AddSystemMessage($"{item.itemName} cannot be equipped.");
            return false;
        }

        if (item.equipmentSlot == EquipmentSlotType.None)
        {
            ChatManager.Instance?.AddSystemMessage($"{item.itemName} has no valid equipment slot.");
            return false;
        }

        if (inventory == null)
        {
            Debug.LogWarning("PlayerInventory missing from player.");
            return false;
        }

        ItemDefinition currentlyEquipped = GetEquippedItem(item.equipmentSlot);

        if (currentlyEquipped != null && !inventory.HasSpaceFor(currentlyEquipped))
        {
            ChatManager.Instance?.AddSystemMessage("Your inventory is full.");
            return false;
        }

        bool removedNewItem = inventory.RemoveItem(item, 1, false);

        if (!removedNewItem)
        {
            return false;
        }

        if (currentlyEquipped != null)
        {
            inventory.AddItem(currentlyEquipped, 1, false);
        }

        equippedItems[item.equipmentSlot] = item;

        RecalculateStats();

        inventory.SaveInventory();
        SaveEquipment();

        ChatManager.Instance?.AddSystemMessage($"You equipped {item.itemName}.");

        OnEquipmentChanged?.Invoke();
        return true;
    }

    public void LoadEquipmentFromSave()
    {
        equippedItems.Clear();

        CharacterData characterData = CharacterSaveManager.LoadCharacter();

        if (characterData == null || characterData.EquippedItems == null)
        {
            RecalculateStats();
            OnEquipmentChanged?.Invoke();
            return;
        }

        foreach (SavedEquippedItem savedItem in characterData.EquippedItems)
        {
            if (savedItem == null)
            {
                continue;
            }

            if (!Enum.TryParse(savedItem.SlotType, out EquipmentSlotType slotType))
            {
                Debug.LogWarning($"Could not parse equipment slot: {savedItem.SlotType}");
                continue;
            }

            ItemDefinition item = ItemDatabase.Instance?.GetItemById(savedItem.ItemId);

            if (item == null)
            {
                Debug.LogWarning($"Could not load equipped item with ID: {savedItem.ItemId}");
                continue;
            }

            equippedItems[slotType] = item;
        }

        RecalculateStats();
        OnEquipmentChanged?.Invoke();
    }

    public void SaveEquipment()
    {
        CharacterData characterData = CharacterSaveManager.LoadCharacter();

        if (characterData == null)
        {
            return;
        }

        characterData.EquippedItems.Clear();

        foreach (KeyValuePair<EquipmentSlotType, ItemDefinition> pair in equippedItems)
        {
            if (pair.Value == null || string.IsNullOrWhiteSpace(pair.Value.itemId))
            {
                continue;
            }

            characterData.EquippedItems.Add(
                new SavedEquippedItem(
                    pair.Key.ToString(),
                    pair.Value.itemId
                )
            );
        }

        CharacterSaveManager.SaveCharacter(characterData);
    }

    private void RecalculateStats()
    {
        BonusDamage = 0;
        BonusMaxHealth = 0;

        foreach (ItemDefinition item in equippedItems.Values)
        {
            if (item == null)
            {
                continue;
            }

            BonusDamage += item.bonusDamage;
            BonusMaxHealth += item.bonusMaxHealth;
        }
    }
}