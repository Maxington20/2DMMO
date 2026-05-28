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
            objectiveText.text = $"Objective: Defeat {quest.requiredKills} {quest.targetEnemyName}";
        }

        if (rewardText != null)
        {
            string reward = $"Reward: {quest.xpReward} XP";

            if (quest.copperReward > 0)
            {
                reward += $" and {quest.copperReward} copper";
            }

            rewardText.text = reward;
        }

        QuestStatus status = questLog.GetQuestStatus(quest);

        bool canAccept = status == QuestStatus.NotAccepted;
        bool canComplete = status == QuestStatus.ReadyToTurnIn;

        if (acceptButton != null)
        {
            acceptButton.gameObject.SetActive(canAccept);
        }

        if (completeButton != null)
        {
            completeButton.gameObject.SetActive(canComplete);
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
        if (currentQuestGiver == null)
        {
            return;
        }

        currentQuestGiver.AcceptQuestFromWindow();
        Hide();
    }

    private void CompleteQuest()
    {
        if (currentQuestGiver == null)
        {
            return;
        }

        currentQuestGiver.CompleteQuestFromWindow();
        Hide();
    }
}