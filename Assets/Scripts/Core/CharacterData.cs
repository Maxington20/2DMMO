using System;

[Serializable]
public class CharacterData
{
    public string CharacterName;
    public string Species;
    public string ClassName;
    public int Level;
    public string StartingZone;

    public CharacterData(string characterName, string species, string className)
    {
        CharacterName = characterName;
        Species = species;
        ClassName = className;
        Level = 1;
        StartingZone = "Frontier's Wake";
    }
}