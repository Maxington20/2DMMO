using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    private readonly Dictionary<EquipmentSlotType, ItemDefinition> equippedItems = new();

    public event Action OnEquipmentChanged;

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

        equippedItems[item.equipmentSlot] = item;

        Debug.Log($"Equipped {item.itemName} in {item.equipmentSlot} slot.");

        OnEquipmentChanged?.Invoke();
        return true;
    }
}