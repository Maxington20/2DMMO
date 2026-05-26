using System.IO;
using UnityEngine;

public static class CharacterSaveManager
{
    private const string SaveFilePrefix = "character_slot_";
    private const string SaveFileExtension = ".json";
    private const string SelectedSlotKey = "SelectedCharacterSlot";

    public const int MaxCharacterSlots = 3;

    private static string GetSavePath(int slotIndex)
    {
        return Path.Combine(
            Application.persistentDataPath,
            $"{SaveFilePrefix}{slotIndex}{SaveFileExtension}"
        );
    }

    public static void SaveCharacter(CharacterData characterData)
    {
        SaveCharacter(GetSelectedSlot(), characterData);
    }

    public static void SaveCharacter(int slotIndex, CharacterData characterData)
    {
        if (characterData == null)
        {
            return;
        }

        if (!IsValidSlot(slotIndex))
        {
            Debug.LogWarning($"Invalid character slot: {slotIndex}");
            return;
        }

        string json = JsonUtility.ToJson(characterData, true);
        File.WriteAllText(GetSavePath(slotIndex), json);

        Debug.Log($"Character slot {slotIndex} saved to: {GetSavePath(slotIndex)}");
    }

    public static CharacterData LoadCharacter()
    {
        return LoadCharacter(GetSelectedSlot());
    }

    public static CharacterData LoadCharacter(int slotIndex)
    {
        if (!IsValidSlot(slotIndex))
        {
            return null;
        }

        string savePath = GetSavePath(slotIndex);

        if (!File.Exists(savePath))
        {
            return null;
        }

        string json = File.ReadAllText(savePath);
        return JsonUtility.FromJson<CharacterData>(json);
    }

    public static bool HasSavedCharacter(int slotIndex)
    {
        if (!IsValidSlot(slotIndex))
        {
            return false;
        }

        return File.Exists(GetSavePath(slotIndex));
    }

    public static void DeleteSavedCharacter(int slotIndex)
    {
        if (!IsValidSlot(slotIndex))
        {
            return;
        }

        string savePath = GetSavePath(slotIndex);

        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
    }

    public static void SetSelectedSlot(int slotIndex)
    {
        if (!IsValidSlot(slotIndex))
        {
            Debug.LogWarning($"Invalid selected slot: {slotIndex}");
            return;
        }

        PlayerPrefs.SetInt(SelectedSlotKey, slotIndex);
        PlayerPrefs.Save();
    }

    public static int GetSelectedSlot()
    {
        int selectedSlot = PlayerPrefs.GetInt(SelectedSlotKey, 0);

        if (!IsValidSlot(selectedSlot))
        {
            return 0;
        }

        return selectedSlot;
    }

    private static bool IsValidSlot(int slotIndex)
    {
        return slotIndex >= 0 && slotIndex < MaxCharacterSlots;
    }
}