using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class QuestGiver : MonoBehaviour
{
    [Header("NPC")]
    [SerializeField] private string displayName = "Thane";

    [Header("Quest")]
    [SerializeField] private QuestDefinition questDefinition;

    [Header("Interaction")]
    [SerializeField] private float interactionRange = 2f;

    private PlayerQuestLog playerQuestLog;

    public string DisplayName => displayName;
    public QuestDefinition QuestDefinition => questDefinition;

    private void Start()
    {
        TryFindQuestLog();
    }

    private void Update()
    {
        if (GameplayInputLock.ShouldBlockWorldClick())
        {
            return;
        }

        Mouse mouse = Mouse.current;

        if (mouse == null || !mouse.leftButton.wasPressedThisFrame)
        {
            return;
        }

        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            return;
        }

        Vector2 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouse.position.ReadValue());
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero);

        if (hit.collider == null || hit.collider.gameObject != gameObject)
        {
            return;
        }

        TryInteract();
    }

    public QuestIndicatorState GetIndicatorState()
    {
        if (questDefinition == null)
        {
            return QuestIndicatorState.None;
        }

        if (playerQuestLog == null)
        {
            TryFindQuestLog();
        }

        if (playerQuestLog == null)
        {
            return QuestIndicatorState.None;
        }

        if (playerQuestLog.IsReadyToTurnIn)
        {
            return QuestIndicatorState.ReadyToTurnIn;
        }

        if (!playerQuestLog.HasActiveQuest && playerQuestLog.Status != QuestStatus.Completed)
        {
            return QuestIndicatorState.Available;
        }

        return QuestIndicatorState.None;
    }

    public void AcceptQuestFromWindow()
    {
        if (playerQuestLog == null)
        {
            TryFindQuestLog();
        }

        if (playerQuestLog == null)
        {
            return;
        }

        playerQuestLog.AcceptQuest(questDefinition);
    }

    public void CompleteQuestFromWindow()
    {
        if (playerQuestLog == null)
        {
            TryFindQuestLog();
        }

        if (playerQuestLog == null)
        {
            return;
        }

        playerQuestLog.TurnInQuest();
    }

    private void TryInteract()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            return;
        }

        float distance = Vector2.Distance(transform.position, playerObject.transform.position);

        if (distance > interactionRange)
        {
            ChatManager.Instance?.AddSystemMessage("You are too far away.");
            return;
        }

        PlayerQuestLog questLog = playerObject.GetComponent<PlayerQuestLog>();

        if (questLog == null)
        {
            return;
        }

        playerQuestLog = questLog;

        QuestInteractionWindow.Instance?.Show(this, questLog);
    }

    private void TryFindQuestLog()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerQuestLog = playerObject.GetComponent<PlayerQuestLog>();
        }
    }
}