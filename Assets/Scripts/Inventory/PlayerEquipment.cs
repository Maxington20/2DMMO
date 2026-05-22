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
            Debug.Log($"{item.itemName} is not equipment.");
            return false;
        }

        if (item.equipmentSlot == EquipmentSlotType.None)
        {
            Debug.LogWarning($"{item.itemName} has no equipment slot assigned.");
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
            Debug.Log("Cannot equip because inventory has no room for the currently equipped item.");
            return false;
        }

        bool removedNewItem = inventory.RemoveItem(item, 1);

        if (!removedNewItem)
        {
            Debug.LogWarning($"Could not remove {item.itemName} from inventory.");
            return false;
        }

        if (currentlyEquipped != null)
        {
            inventory.AddItem(currentlyEquipped, 1);
        }

        equippedItems[item.equipmentSlot] = item;

        RecalculateStats();

        Debug.Log($"Equipped {item.itemName} in {item.equipmentSlot} slot.");

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