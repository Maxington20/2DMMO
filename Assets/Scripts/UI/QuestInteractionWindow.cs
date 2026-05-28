using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestInteractionWindow : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject windowRoot;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI npcNameText;
    [SerializeField] private TextMeshProUGUI questTitleText;
    [SerializeField] private TextMeshProUGUI questDescriptionText;
    [SerializeField] private TextMeshProUGUI objectiveText;
    [SerializeField] private TextMeshProUGUI rewardText;

    [Header("Buttons")]
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button completeButton;
    [SerializeField] private Button closeButton;

    private QuestGiver currentQuestGiver;

    public static QuestInteractionWindow Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        if (acceptButton != null)
        {
            acceptButton.onClick.AddListener(AcceptQuest);
        }

        if (completeButton != null)
        {
            completeButton.onClick.AddListener(CompleteQuest);
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Hide);
        }

        Hide();
    }

    private void OnDestroy()
    {
        GameplayInputLock.ClearLock(this);
    }

    public void Show(QuestGiver questGiver, PlayerQuestLog questLog)
    {
        if (questGiver == null || questLog == null)
        {
            return;
        }

        currentQuestGiver = questGiver;

        QuestDefinition quest = questGiver.QuestDefinition;

        if (quest == null)
        {
            return;
        }

        windowRoot.SetActive(true);
        GameplayInputLock.SetLocked(this, true);

        if (npcNameText != null)
        {
            npcNameText.text = questGiver.DisplayName;
        }

        if (questTitleText != null)
        {
            questTitleText.text = quest.questName;
        }

        if (questDescriptionText != null)
        {
            questDescriptionText.text = quest.description;
        }

        if (objectiveText != null)
        {
            objectiveText.text = $"Objective:\nDefeat {quest.requiredKills} {quest.targetEnemyName}";
        }

        if (rewardText != null)
        {
            string rewards = "<b>Rewards</b>\n";

            if (quest.xpReward > 0)
            {
                rewards += $"{quest.xpReward} XP\n";
            }

            if (quest.copperReward > 0)
            {
                rewards += $"{quest.copperReward} Copper\n";
            }

            rewardText.text = rewards.TrimEnd();
        }

        QuestStatus status = questLog.GetQuestStatus(quest);

        if (acceptButton != null)
        {
            acceptButton.gameObject.SetActive(status == QuestStatus.NotAccepted);
        }

        if (completeButton != null)
        {
            completeButton.gameObject.SetActive(status == QuestStatus.ReadyToTurnIn);
        }
    }

    public void Hide()
    {
        if (windowRoot != null)
        {
            windowRoot.SetActive(false);
        }

        GameplayInputLock.SetLocked(this, false);
        currentQuestGiver = null;
    }

    private void AcceptQuest()
    {
        currentQuestGiver?.AcceptQuestFromWindow();
        Hide();
    }

    private void CompleteQuest()
    {
        ChatManager.Instance?.AddSystemMessage("DEBUG: Complete button clicked.");

        currentQuestGiver?.CompleteQuestFromWindow();
        Hide();
    }
}