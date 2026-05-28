using System.Collections.Generic;
using System.Text;
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

        List<QuestRuntimeState> visibleQuests = questLog.GetVisibleQuests();

        if (visibleQuests.Count == 0)
        {
            trackerPanel.SetActive(false);
            return;
        }

        trackerPanel.SetActive(true);

        if (titleText != null)
        {
            titleText.text = visibleQuests.Count == 1 ? visibleQuests[0].Quest.questName : "Quests";
        }

        if (objectiveText != null)
        {
            StringBuilder builder = new();

            foreach (QuestRuntimeState state in visibleQuests)
            {
                if (state == null || state.Quest == null)
                {
                    continue;
                }

                if (visibleQuests.Count > 1)
                {
                    builder.AppendLine($"<b>{state.Quest.questName}</b>");
                }

                if (state.Status == QuestStatus.ReadyToTurnIn)
                {
                    builder.AppendLine("Return to quest giver");
                }
                else
                {
                    builder.AppendLine($"Defeat {state.Quest.targetEnemyName}: {state.CurrentKills} / {state.Quest.requiredKills}");
                }

                builder.AppendLine();
            }

            objectiveText.text = builder.ToString().TrimEnd();
        }
    }
}