using System.IO;
using UnityEngine;

public static class CharacterSaveManager
{
    private const string SaveFileName = "character_save.json";

    private static string SavePath =>
        Path.Combine(Application.persistentDataPath, SaveFileName);

    public static void SaveCharacter(CharacterData characterData)
    {
        if (characterData == null)
        {
            return;
        }

        string json = JsonUtility.ToJson(characterData, true);
        File.WriteAllText(SavePath, json);

        Debug.Log($"Character saved to: {SavePath}");
    }

    public static CharacterData LoadCharacter()
    {
        if (!File.Exists(SavePath))
        {
            return null;
        }

        string json = File.ReadAllText(SavePath);
        return JsonUtility.FromJson<CharacterData>(json);
    }

    public static bool HasSavedCharacter()
    {
        return File.Exists(SavePath);
    }

    public static void DeleteSavedCharacter()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
        }
    }
}