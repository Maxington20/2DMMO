using System;

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