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

        bool removedNewItem = inventory.RemoveItem(item, 1);

        if (!removedNewItem)
        {
            return false;
        }

        if (currentlyEquipped != null)
        {
            inventory.AddItem(currentlyEquipped, 1);
        }

        equippedItems[item.equipmentSlot] = item;

        RecalculateStats();

        ChatManager.Instance?.AddSystemMessage($"You equipped {item.itemName}.");

        OnEquipmentChanged?.Invoke();
        return true;
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