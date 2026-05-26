using System;
using UnityEngine;

public class PlayerQuestLog : MonoBehaviour
{
    [Header("Available Quest")]
    [SerializeField] private QuestDefinition starterQuest;

    [Header("Runtime Quest")]
    [SerializeField] private QuestDefinition activeQuest;

    private int currentKills;
    private QuestStatus status = QuestStatus.NotAccepted;

    public QuestDefinition ActiveQuest => activeQuest;
    public int CurrentKills => currentKills;
    public QuestStatus Status => status;

    public event Action OnQuestUpdated;

    public bool HasActiveQuest => activeQuest != null && status == QuestStatus.InProgress;
    public bool IsReadyToTurnIn => activeQuest != null && status == QuestStatus.ReadyToTurnIn;
    public bool HasCompletedStarterQuest => status == QuestStatus.Completed;

    private void Start()
    {
        LoadQuestProgress();
    }

    public void AcceptQuest(QuestDefinition quest)
    {
        if (quest == null)
        {
            return;
        }

        if (activeQuest != null && status != QuestStatus.Completed)
        {
            ChatManager.Instance?.AddSystemMessage("You already have an active quest.");
            return;
        }

        activeQuest = quest;
        currentKills = 0;
        status = QuestStatus.InProgress;

        ChatManager.Instance?.AddSystemMessage($"Quest accepted: {quest.questName}");

        SaveQuestProgress();
        OnQuestUpdated?.Invoke();
    }

    public void RegisterEnemyKill(Enemy enemy)
    {
        if (enemy == null || activeQuest == null || status != QuestStatus.InProgress)
        {
            return;
        }

        if (enemy.EnemyName != activeQuest.targetEnemyName)
        {
            return;
        }

        currentKills++;

        ChatManager.Instance?.AddSystemMessage(
            $"{activeQuest.questName}: {currentKills}/{activeQuest.requiredKills} {activeQuest.targetEnemyName} defeated."
        );

        if (currentKills >= activeQuest.requiredKills)
        {
            currentKills = activeQuest.requiredKills;
            status = QuestStatus.ReadyToTurnIn;

            ChatManager.Instance?.AddSystemMessage($"Quest complete: {activeQuest.questName}. Return to the quest giver.");
        }

        SaveQuestProgress();
        OnQuestUpdated?.Invoke();
    }

    public void TurnInQuest()
    {
        if (activeQuest == null || status != QuestStatus.ReadyToTurnIn)
        {
            return;
        }

        PlayerProgression progression = GetComponent<PlayerProgression>();

        if (progression != null && activeQuest.xpReward > 0)
        {
            progression.AddXp(activeQuest.xpReward);
        }

        ChatManager.Instance?.AddSystemMessage($"Quest turned in: {activeQuest.questName}");

        status = QuestStatus.Completed;
        currentKills = 0;

        SaveQuestProgress();

        activeQuest = null;

        OnQuestUpdated?.Invoke();
    }

    private void LoadQuestProgress()
    {
        CharacterData characterData = CharacterSaveManager.LoadCharacter();

        if (characterData == null || characterData.QuestProgress == null)
        {
            return;
        }

        SavedQuestProgress savedQuest = characterData.QuestProgress;

        if (starterQuest == null || starterQuest.questId != savedQuest.QuestId)
        {
            return;
        }

        if (!Enum.TryParse(savedQuest.Status, out QuestStatus loadedStatus))
        {
            loadedStatus = QuestStatus.NotAccepted;
        }

        status = loadedStatus;
        currentKills = Mathf.Clamp(savedQuest.CurrentKills, 0, starterQuest.requiredKills);

        if (status == QuestStatus.InProgress || status == QuestStatus.ReadyToTurnIn)
        {
            activeQuest = starterQuest;
        }
        else
        {
            activeQuest = null;
        }

        OnQuestUpdated?.Invoke();
    }

    private void SaveQuestProgress()
    {
        CharacterData characterData = CharacterSaveManager.LoadCharacter();

        if (characterData == null)
        {
            return;
        }

        QuestDefinition questToSave = activeQuest != null ? activeQuest : starterQuest;

        if (questToSave == null || string.IsNullOrWhiteSpace(questToSave.questId))
        {
            return;
        }

        characterData.QuestProgress = new SavedQuestProgress(
            questToSave.questId,
            currentKills,
            status.ToString()
        );

        CharacterSaveManager.SaveCharacter(characterData);
    }
}