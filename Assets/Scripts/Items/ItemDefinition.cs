using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "MMO Simulator/Item")]
public class ItemDefinition : ScriptableObject
{
    [Header("Item Info")]
    public string itemName;
    public ItemRarity rarity;
    public Sprite icon;
    public int sellValue;
}