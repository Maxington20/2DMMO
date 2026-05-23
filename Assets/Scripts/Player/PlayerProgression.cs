using UnityEngine;

public class PlayerProgression : MonoBehaviour
{
    [Header("Level")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int currentXp = 0;
    [SerializeField] private int xpToNextLevel = 100;

    [Header("Scaling")]
    [SerializeField] private int healthIncreasePerLevel = 10;

    private Health health;

    public int CurrentLevel => currentLevel;
    public int CurrentXp => currentXp;
    public int XpToNextLevel => xpToNextLevel;

    private void Awake()
    {
        health = GetComponent<Health>();
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
}