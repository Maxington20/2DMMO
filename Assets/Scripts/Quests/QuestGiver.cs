using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class QuestGiver : MonoBehaviour
{
    [Header("Quest")]
    [SerializeField] private QuestDefinition questDefinition;

    [Header("Interaction")]
    [SerializeField] private float interactionRange = 2f;

    private void Update()
    {
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

        if (questLog.IsReadyToTurnIn)
        {
            questLog.TurnInQuest();
            return;
        }

        if (!questLog.HasActiveQuest)
        {
            questLog.AcceptQuest(questDefinition);
            return;
        }

        ChatManager.Instance?.AddSystemMessage("You are already working on a quest.");
    }
}