using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "MMO Simulator/Item")]
public class ItemDefinition : ScriptableObject
{
    [Header("Save Identity")]
    public string itemId;

    [Header("Item Info")]
    public string itemName;
    public ItemRarity rarity;
    public Sprite icon;
    public int sellValue;

    [Header("Item Type")]
    public ItemType itemType = ItemType.Junk;
    public EquipmentSlotType equipmentSlot = EquipmentSlotType.None;

    [Header("Equipment Stats")]
    public int bonusMaxHealth;
    public int bonusDamage;
}