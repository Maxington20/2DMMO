using System;
using System.Collections.Generic;

[Serializable]
public class CharacterData
{
    public string CharacterName;
    public string Species;
    public string ClassName;
    public int Level;
    public int CurrentXp;
    public int XpToNextLevel;
    public string StartingZone;

    public List<SavedInventoryItem> InventoryItems = new();
    public List<SavedEquippedItem> EquippedItems = new();

    public CharacterData(string characterName, string species, string className)
    {
        CharacterName = characterName;
        Species = species;
        ClassName = className;
        Level = 1;
        CurrentXp = 0;
        XpToNextLevel = 100;
        StartingZone = "Frontier's Wake";
    }
}

[Serializable]
public class SavedInventoryItem
{
    public string ItemId;
    public int Quantity;

    public SavedInventoryItem(string itemId, int quantity)
    {
        ItemId = itemId;
        Quantity = quantity;
    }
}

[Serializable]
public class SavedEquippedItem
{
    public string SlotType;
    public string ItemId;

    public SavedEquippedItem(string slotType, string itemId)
    {
        SlotType = slotType;
        ItemId = itemId;
    }
}