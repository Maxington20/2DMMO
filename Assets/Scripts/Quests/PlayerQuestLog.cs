using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerQuestLog : MonoBehaviour
{
    [Header("Available Quests")]
    [SerializeField] private QuestDefinition[] availableQuests;

    private readonly Dictionary<string, QuestRuntimeState> questStates = new();

    public event Action OnQuestUpdated;

    public IReadOnlyDictionary<string, QuestRuntimeState> QuestStates => questStates;

    private void Start()
    {
        LoadQuestProgress();
        OnQuestUpdated?.Invoke();
    }

    public QuestStatus GetQuestStatus(QuestDefinition quest)
    {
        if (quest == null || string.IsNullOrWhiteSpace(quest.questId))
        {
            return QuestStatus.NotAccepted;
        }

        if (!questStates.TryGetValue(quest.questId, out QuestRuntimeState state))
        {
            return QuestStatus.NotAccepted;
        }

        return state.Status;
    }

    public int GetCurrentKills(QuestDefinition quest)
    {
        if (quest == null || string.IsNullOrWhiteSpace(quest.questId))
        {
            return 0;
        }

        if (!questStates.TryGetValue(quest.questId, out QuestRuntimeState state))
        {
            return 0;
        }

        return state.CurrentKills;
    }

    public bool HasActiveQuest => HasAnyQuestWithStatus(QuestStatus.InProgress);
    public bool IsReadyToTurnIn => HasAnyQuestWithStatus(QuestStatus.ReadyToTurnIn);

    public bool CanAcceptQuest(QuestDefinition quest)
    {
        return quest != null && GetQuestStatus(quest) == QuestStatus.NotAccepted;
    }

    public bool CanTurnInQuest(QuestDefinition quest)
    {
        return quest != null && GetQuestStatus(quest) == QuestStatus.ReadyToTurnIn;
    }

    public void AcceptQuest(QuestDefinition quest)
    {
        if (quest == null || string.IsNullOrWhiteSpace(quest.questId))
        {
            return;
        }

        if (!CanAcceptQuest(quest))
        {
            ChatManager.Instance?.AddSystemMessage("You cannot accept that quest right now.");
            return;
        }

        questStates[quest.questId] = new QuestRuntimeState
        {
            Quest = quest,
            CurrentKills = 0,
            Status = QuestStatus.InProgress
        };

        ChatManager.Instance?.AddSystemMessage($"Quest accepted: {quest.questName}");

        SaveQuestProgress();
        OnQuestUpdated?.Invoke();
    }

    public void RegisterEnemyKill(Enemy enemy)
    {
        if (enemy == null)
        {
            return;
        }

        bool updated = false;

        foreach (QuestRuntimeState state in questStates.Values)
        {
            if (state == null || state.Quest == null)
            {
                continue;
            }

            if (state.Status != QuestStatus.InProgress)
            {
                continue;
            }

            if (enemy.EnemyName != state.Quest.targetEnemyName)
            {
                continue;
            }

            state.CurrentKills++;
            state.CurrentKills = Mathf.Clamp(state.CurrentKills, 0, state.Quest.requiredKills);

            ChatManager.Instance?.AddSystemMessage(
                $"{state.Quest.questName}: {state.CurrentKills}/{state.Quest.requiredKills} {state.Quest.targetEnemyName} defeated."
            );

            if (state.CurrentKills >= state.Quest.requiredKills)
            {
                state.Status = QuestStatus.ReadyToTurnIn;

                ChatManager.Instance?.AddSystemMessage(
                    $"Quest complete: {state.Quest.questName}. Return to the quest giver."
                );
            }

            updated = true;
        }

        if (updated)
        {
            SaveQuestProgress();
            OnQuestUpdated?.Invoke();
        }
    }

    public void TurnInQuest(QuestDefinition quest)
    {
        if (quest == null || string.IsNullOrWhiteSpace(quest.questId))
        {
            ChatManager.Instance?.AddSystemMessage("Could not turn in quest.");
            return;
        }

        if (!questStates.TryGetValue(quest.questId, out QuestRuntimeState state))
        {
            ChatManager.Instance?.AddSystemMessage("You are not on that quest.");
            return;
        }

        if (state.Status != QuestStatus.ReadyToTurnIn)
        {
            ChatManager.Instance?.AddSystemMessage("That quest is not ready to turn in.");
            return;
        }

        PlayerProgression progression = GetComponent<PlayerProgression>();
        PlayerCurrency currency = GetComponent<PlayerCurrency>();

        ChatManager.Instance?.AddSystemMessage($"Quest turned in: {quest.questName}");

        if (progression != null && quest.xpReward > 0)
        {
            progression.AddXp(quest.xpReward);
        }

        if (currency != null && quest.copperReward > 0)
        {
            currency.AddCopper(quest.copperReward);
            ChatManager.Instance?.AddSystemMessage($"You received {quest.copperReward} copper.");
        }

        state.Status = QuestStatus.Completed;
        state.CurrentKills = quest.requiredKills;

        SaveQuestProgress();
        OnQuestUpdated?.Invoke();
    }

    public List<QuestRuntimeState> GetVisibleQuests()
    {
        List<QuestRuntimeState> visibleQuests = new();

        foreach (QuestRuntimeState state in questStates.Values)
        {
            if (state == null || state.Quest == null)
            {
                continue;
            }

            if (state.Status == QuestStatus.InProgress || state.Status == QuestStatus.ReadyToTurnIn)
            {
                visibleQuests.Add(state);
            }
        }

        return visibleQuests;
    }

    private bool HasAnyQuestWithStatus(QuestStatus targetStatus)
    {
        foreach (QuestRuntimeState state in questStates.Values)
        {
            if (state != null && state.Status == targetStatus)
            {
                return true;
            }
        }

        return false;
    }

    private void LoadQuestProgress()
    {
        questStates.Clear();

        CharacterData characterData = CharacterSaveManager.LoadCharacter();

        if (characterData == null)
        {
            return;
        }

        if (characterData.QuestProgressList == null)
        {
            characterData.QuestProgressList = new List<SavedQuestProgress>();
        }

        if (characterData.QuestProgress != null && !string.IsNullOrWhiteSpace(characterData.QuestProgress.QuestId))
        {
            bool alreadyExists = characterData.QuestProgressList.Exists(q => q.QuestId == characterData.QuestProgress.QuestId);

            if (!alreadyExists)
            {
                characterData.QuestProgressList.Add(characterData.QuestProgress);
            }
        }

        foreach (SavedQuestProgress savedQuest in characterData.QuestProgressList)
        {
            if (savedQuest == null || string.IsNullOrWhiteSpace(savedQuest.QuestId))
            {
                continue;
            }

            QuestDefinition quest = FindAvailableQuest(savedQuest.QuestId);

            if (quest == null)
            {
                continue;
            }

            if (!Enum.TryParse(savedQuest.Status, out QuestStatus loadedStatus))
            {
                loadedStatus = QuestStatus.NotAccepted;
            }

            questStates[quest.questId] = new QuestRuntimeState
            {
                Quest = quest,
                CurrentKills = Mathf.Clamp(savedQuest.CurrentKills, 0, quest.requiredKills),
                Status = loadedStatus
            };
        }

        SaveQuestProgress();
    }

    private void SaveQuestProgress()
    {
        CharacterData characterData = CharacterSaveManager.LoadCharacter();

        if (characterData == null)
        {
            return;
        }

        if (characterData.QuestProgressList == null)
        {
            characterData.QuestProgressList = new List<SavedQuestProgress>();
        }

        characterData.QuestProgressList.Clear();

        foreach (QuestRuntimeState state in questStates.Values)
        {
            if (state == null || state.Quest == null || string.IsNullOrWhiteSpace(state.Quest.questId))
            {
                continue;
            }

            characterData.QuestProgressList.Add(
                new SavedQuestProgress(
                    state.Quest.questId,
                    state.CurrentKills,
                    state.Status.ToString()
                )
            );
        }

        CharacterSaveManager.SaveCharacter(characterData);
    }

    private QuestDefinition FindAvailableQuest(string questId)
    {
        if (availableQuests == null)
        {
            return null;
        }

        foreach (QuestDefinition quest in availableQuests)
        {
            if (quest != null && quest.questId == questId)
            {
                return quest;
            }
        }

        return null;
    }
}

[Serializable]
public class QuestRuntimeState
{
    public QuestDefinition Quest;
    public int CurrentKills;
    public QuestStatus Status;
}