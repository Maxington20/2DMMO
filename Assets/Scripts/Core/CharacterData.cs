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
    public int Copper;

    public List<SavedInventoryItem> InventoryItems = new();
    public List<SavedEquippedItem> EquippedItems = new();

    // Kept for compatibility with older saves.
    public SavedQuestProgress QuestProgress;

    public List<SavedQuestProgress> QuestProgressList = new();

    public CharacterData(string characterName, string species, string className)
    {
        CharacterName = characterName;
        Species = species;
        ClassName = className;
        Level = 1;
        CurrentXp = 0;
        XpToNextLevel = 100;
        StartingZone = "Frontier's Wake";
        Copper = 0;
        QuestProgress = null;
        QuestProgressList = new List<SavedQuestProgress>();
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

[Serializable]
public class SavedQuestProgress
{
    public string QuestId;
    public int CurrentKills;
    public string Status;

    public SavedQuestProgress(string questId, int currentKills, string status)
    {
        QuestId = questId;
        CurrentKills = currentKills;
        Status = status;
    }
}