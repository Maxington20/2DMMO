using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FakePlayerQuestController : MonoBehaviour
{
    private enum FakeQuestState
    {
        TravelingToQuestGiver,
        AcceptingQuest,
        TravelingToHuntArea,
        Hunting,
        ReturningToQuestGiver,
        TurningInQuest,
        Resting
    }

    [Header("Quest")]
    [SerializeField] private QuestDefinition questDefinition;
    [SerializeField] private Transform questGiver;
    [SerializeField] private Transform huntArea;
    [SerializeField] private bool repeatQuest = false;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float arrivalDistance = 0.15f;

    [Header("Timing")]
    [SerializeField] private float acceptDelaySeconds = 1.5f;
    [SerializeField] private float turnInDelaySeconds = 1.5f;
    [SerializeField] private float restDelaySeconds = 8f;
    [SerializeField] private float huntingCommentIntervalMin = 10f;
    [SerializeField] private float huntingCommentIntervalMax = 20f;

    [Header("Chat")]
    [SerializeField] private string fakePlayerName = "Fake Player";
    [SerializeField] private bool announceQuestProgress = true;
    [SerializeField] private bool chatterEnabled = true;

    private Rigidbody2D rb;
    private FakePlayerCombatController combatController;
    private FakePlayerController wanderController;

    private FakeQuestState currentState;
    private int currentKills;
    private float stateTimer;
    private float huntingCommentTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        combatController = GetComponent<FakePlayerCombatController>();
        wanderController = GetComponent<FakePlayerController>();
    }

    private void Start()
    {
        SetState(FakeQuestState.TravelingToQuestGiver);
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case FakeQuestState.TravelingToQuestGiver:
                MoveTowardStateTarget(questGiver, FakeQuestState.AcceptingQuest);
                break;

            case FakeQuestState.TravelingToHuntArea:
                MoveTowardStateTarget(huntArea, FakeQuestState.Hunting);
                break;

            case FakeQuestState.ReturningToQuestGiver:
                MoveTowardStateTarget(questGiver, FakeQuestState.TurningInQuest);
                break;
        }
    }

    private void Update()
    {
        switch (currentState)
        {
            case FakeQuestState.AcceptingQuest:
                TickAcceptingQuest();
                break;

            case FakeQuestState.Hunting:
                TickHunting();
                break;

            case FakeQuestState.TurningInQuest:
                TickTurningInQuest();
                break;

            case FakeQuestState.Resting:
                TickResting();
                break;
        }
    }

    public void RegisterEnemyKill(Enemy enemy)
    {
        if (enemy == null || questDefinition == null)
        {
            return;
        }

        if (currentState != FakeQuestState.Hunting)
        {
            return;
        }

        if (enemy.EnemyName != questDefinition.targetEnemyName)
        {
            return;
        }

        currentKills++;
        currentKills = Mathf.Clamp(currentKills, 0, questDefinition.requiredKills);

        if (announceQuestProgress)
        {
            ChatManager.Instance?.AddMessage(
                ChatChannel.Zone,
                fakePlayerName,
                $"{questDefinition.questName}: {currentKills}/{questDefinition.requiredKills} {questDefinition.targetEnemyName} defeated."
            );
        }

        if (currentKills >= questDefinition.requiredKills)
        {
            SayRandom(new[]
            {
                "done with this one, heading back",
                "quest complete, turning it in",
                "finally done lol",
                "easy enough, going back to town"
            });

            SetState(FakeQuestState.ReturningToQuestGiver);
        }
    }

    private void TickAcceptingQuest()
    {
        stateTimer -= Time.deltaTime;

        if (stateTimer > 0f)
        {
            return;
        }

        currentKills = 0;

        SayRandom(new[]
        {
            $"grabbing {questDefinition.questName}",
            $"picking up {questDefinition.questName}",
            "taking this quest real quick",
            "quest accepted, heading out"
        });

        SetState(FakeQuestState.TravelingToHuntArea);
    }

    private void TickHunting()
    {
        if (combatController != null)
        {
            combatController.enabled = true;
        }

        huntingCommentTimer -= Time.deltaTime;

        if (huntingCommentTimer <= 0f)
        {
            SayRandom(new[]
            {
                $"these {questDefinition.targetEnemyName.ToLower()}s are everywhere",
                "pulling another one",
                "almost done here",
                "this spawn rate is actually decent",
                "anyone else doing this quest?"
            });

            ResetHuntingCommentTimer();
        }
    }

    private void TickTurningInQuest()
    {
        stateTimer -= Time.deltaTime;

        if (stateTimer > 0f)
        {
            return;
        }

        SayRandom(new[]
        {
            $"turned in {questDefinition.questName}",
            "nice, free XP",
            "quest done",
            "easy turn-in"
        });

        if (questDefinition.xpReward > 0)
        {
            ChatManager.Instance?.AddMessage(
                ChatChannel.Zone,
                fakePlayerName,
                $"gained {questDefinition.xpReward} XP."
            );
        }

        SetState(FakeQuestState.Resting);
    }

    private void TickResting()
    {
        stateTimer -= Time.deltaTime;

        if (stateTimer > 0f)
        {
            return;
        }

        if (repeatQuest)
        {
            SetState(FakeQuestState.TravelingToQuestGiver);
            return;
        }

        SayRandom(new[]
        {
            "gonna wander for a bit",
            "taking a break from questing",
            "back to town stuff",
            "might do another quest later"
        });

        if (wanderController != null)
        {
            wanderController.enabled = true;
        }

        if (combatController != null)
        {
            combatController.enabled = false;
        }

        enabled = false;
    }

    private void MoveTowardStateTarget(Transform target, FakeQuestState nextState)
    {
        if (target == null)
        {
            return;
        }

        Vector2 currentPosition = rb.position;
        Vector2 targetPosition = target.position;
        Vector2 direction = targetPosition - currentPosition;

        if (direction.magnitude <= arrivalDistance)
        {
            rb.MovePosition(targetPosition);
            SetState(nextState);
            return;
        }

        Vector2 nextPosition =
            currentPosition + direction.normalized * moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(nextPosition);
    }

    private void SetState(FakeQuestState newState)
    {
        currentState = newState;

        bool shouldUseQuestMovement =
            currentState == FakeQuestState.TravelingToQuestGiver ||
            currentState == FakeQuestState.TravelingToHuntArea ||
            currentState == FakeQuestState.ReturningToQuestGiver;

        bool shouldUseCombat =
            currentState == FakeQuestState.Hunting;

        if (wanderController != null)
        {
            wanderController.enabled = !shouldUseQuestMovement && !shouldUseCombat;
        }

        if (combatController != null)
        {
            combatController.enabled = shouldUseCombat;
        }

        switch (currentState)
        {
            case FakeQuestState.AcceptingQuest:
                stateTimer = acceptDelaySeconds;
                break;

            case FakeQuestState.Hunting:
                ResetHuntingCommentTimer();
                break;

            case FakeQuestState.TurningInQuest:
                stateTimer = turnInDelaySeconds;
                break;

            case FakeQuestState.Resting:
                stateTimer = restDelaySeconds;
                break;
        }
    }

    private void ResetHuntingCommentTimer()
    {
        huntingCommentTimer = Random.Range(huntingCommentIntervalMin, huntingCommentIntervalMax);
    }

    private void SayRandom(string[] messages)
    {
        if (!chatterEnabled || messages == null || messages.Length == 0)
        {
            return;
        }

        string message = messages[Random.Range(0, messages.Length)];

        ChatManager.Instance?.AddMessage(
            ChatChannel.Zone,
            fakePlayerName,
            message
        );
    }
}