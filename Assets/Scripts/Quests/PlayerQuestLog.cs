using System;
using UnityEngine;

public class PlayerQuestLog : MonoBehaviour
{
    [Header("Quest")]
    [SerializeField] private QuestDefinition activeQuest;

    private int currentKills;
    private QuestStatus status = QuestStatus.NotAccepted;

    public QuestDefinition ActiveQuest => activeQuest;
    public int CurrentKills => currentKills;
    public QuestStatus Status => status;

    public event Action OnQuestUpdated;

    public bool HasActiveQuest => activeQuest != null && status == QuestStatus.InProgress;
    public bool IsReadyToTurnIn => activeQuest != null && status == QuestStatus.ReadyToTurnIn;

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
        activeQuest = null;
        currentKills = 0;

        OnQuestUpdated?.Invoke();
    }
}