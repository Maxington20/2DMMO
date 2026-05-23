using TMPro;
using UnityEngine;

public class QuestTrackerUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject trackerPanel;
    [SerializeField] private PlayerQuestLog questLog;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI objectiveText;

    private void Start()
    {
        if (questLog != null)
        {
            questLog.OnQuestUpdated += Refresh;
        }

        Refresh();
    }

    private void OnDestroy()
    {
        if (questLog != null)
        {
            questLog.OnQuestUpdated -= Refresh;
        }
    }

    private void Refresh()
    {
        if (trackerPanel == null || questLog == null)
        {
            return;
        }

        QuestDefinition quest = questLog.ActiveQuest;

        if (quest == null || questLog.Status == QuestStatus.Completed)
        {
            trackerPanel.SetActive(false);
            return;
        }

        trackerPanel.SetActive(true);

        if (titleText != null)
        {
            titleText.text = quest.questName;
        }

        if (objectiveText != null)
        {
            string statusText = questLog.Status == QuestStatus.ReadyToTurnIn
                ? "Return to Thane"
                : $"Defeat {quest.targetEnemyName}: {questLog.CurrentKills} / {quest.requiredKills}";

            objectiveText.text = statusText;
        }
    }
}