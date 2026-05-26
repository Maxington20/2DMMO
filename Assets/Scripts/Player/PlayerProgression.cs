using UnityEngine;

public class PlayerProgression : MonoBehaviour
{
    [Header("Level")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int currentXp = 0;
    [SerializeField] private int xpToNextLevel = 100;

    [Header("Scaling")]
    [SerializeField] private int healthIncreasePerLevel = 10;

    private const int DefaultXpToNextLevel = 100;

    private Health health;

    public int CurrentLevel => currentLevel;
    public int CurrentXp => currentXp;
    public int XpToNextLevel => xpToNextLevel;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void Start()
    {
        LoadProgressionFromSave();
    }

    public void AddXp(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        currentXp += amount;

        ChatManager.Instance?.AddSystemMessage($"You gained {amount} XP.");

        while (currentXp >= xpToNextLevel)
        {
            currentXp -= xpToNextLevel;
            LevelUp();
        }

        SaveProgression();
    }

    private void LevelUp()
    {
        currentLevel++;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.25f);

        if (health != null)
        {
            health.IncreaseMaxHealth(healthIncreasePerLevel, true);
        }

        ChatManager.Instance?.AddSystemMessage($"Congratulations! You reached level {currentLevel}.");
    }

    private void LoadProgressionFromSave()
    {
        CharacterData characterData = CharacterSaveManager.LoadCharacter();

        if (characterData == null)
        {
            currentLevel = 1;
            currentXp = 0;
            xpToNextLevel = DefaultXpToNextLevel;
            return;
        }

        currentLevel = Mathf.Max(1, characterData.Level);
        currentXp = Mathf.Max(0, characterData.CurrentXp);

        xpToNextLevel = characterData.XpToNextLevel <= 1
            ? DefaultXpToNextLevel
            : characterData.XpToNextLevel;

        SaveProgression();
    }

    private void SaveProgression()
    {
        CharacterData characterData = CharacterSaveManager.LoadCharacter();

        if (characterData == null)
        {
            return;
        }

        characterData.Level = currentLevel;
        characterData.CurrentXp = currentXp;
        characterData.XpToNextLevel = xpToNextLevel;

        CharacterSaveManager.SaveCharacter(characterData);
    }
}