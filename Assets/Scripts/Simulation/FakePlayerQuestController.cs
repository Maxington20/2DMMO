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

    [Header("Fake Player Identity")]
    [SerializeField] private string fakePlayerName = "Fake Player";
    [SerializeField] private FakePlayerPersonalityType personalityType = FakePlayerPersonalityType.Casual;

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
            Say(GetProgressMessage());
        }

        if (currentKills >= questDefinition.requiredKills)
        {
            SayRandom(GetQuestCompleteMessages());
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

        SayRandom(GetAcceptQuestMessages());

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
            SayRandom(GetHuntingMessages());
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

        SayRandom(GetTurnInMessages());

        if (questDefinition.xpReward > 0)
        {
            Say(GetXpMessage());
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

        SayRandom(GetRestMessages());

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

        Vector2 nextPosition = currentPosition + direction.normalized * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(nextPosition);
    }

    private void SetState(FakeQuestState newState)
    {
        currentState = newState;

        bool shouldUseQuestMovement =
            currentState == FakeQuestState.TravelingToQuestGiver ||
            currentState == FakeQuestState.TravelingToHuntArea ||
            currentState == FakeQuestState.ReturningToQuestGiver;

        bool shouldUseCombat = currentState == FakeQuestState.Hunting;

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
        if (messages == null || messages.Length == 0)
        {
            return;
        }

        Say(messages[Random.Range(0, messages.Length)]);
    }

    private void Say(string message)
    {
        if (!chatterEnabled || string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        ChatManager.Instance?.AddMessage(ChatChannel.Zone, fakePlayerName, message);
    }

    private string GetProgressMessage()
    {
        return personalityType switch
        {
            FakePlayerPersonalityType.Helpful =>
                $"{questDefinition.questName}: {currentKills}/{questDefinition.requiredKills} done if anyone is tracking",

            FakePlayerPersonalityType.Grumpy =>
                $"{currentKills}/{questDefinition.requiredKills}. this is taking forever",

            FakePlayerPersonalityType.Tryhard =>
                $"{currentKills}/{questDefinition.requiredKills}. efficient route so far",

            FakePlayerPersonalityType.Newbie =>
                $"i got {currentKills}/{questDefinition.requiredKills}, is that good?",

            _ =>
                $"{questDefinition.questName}: {currentKills}/{questDefinition.requiredKills} {questDefinition.targetEnemyName} defeated."
        };
    }

    private string GetXpMessage()
    {
        return personalityType switch
        {
            FakePlayerPersonalityType.Helpful => $"got {questDefinition.xpReward} XP from that quest",
            FakePlayerPersonalityType.Grumpy => $"only {questDefinition.xpReward} XP? alright whatever",
            FakePlayerPersonalityType.Tryhard => $"+{questDefinition.xpReward} XP. moving on",
            FakePlayerPersonalityType.Newbie => $"wait nice, i got {questDefinition.xpReward} XP",
            _ => $"gained {questDefinition.xpReward} XP."
        };
    }

    private string[] GetAcceptQuestMessages()
    {
        return personalityType switch
        {
            FakePlayerPersonalityType.Helpful => new[]
            {
                $"picking up {questDefinition.questName} if anyone wants to group",
                $"grabbing {questDefinition.questName}, objective is {questDefinition.targetEnemyName}s",
                "heading out for quest mobs now"
            },

            FakePlayerPersonalityType.Grumpy => new[]
            {
                $"ugh, doing {questDefinition.questName}",
                "fine, grabbing this quest",
                "hope the drop/spawn rate isn't trash"
            },

            FakePlayerPersonalityType.Tryhard => new[]
            {
                $"taking {questDefinition.questName}, should be quick",
                "starting quest route",
                "pulling fast, don't stand in my way"
            },

            FakePlayerPersonalityType.Newbie => new[]
            {
                $"i think i accepted {questDefinition.questName}",
                "where do i go for this quest?",
                $"do i just kill {questDefinition.targetEnemyName}s?"
            },

            _ => new[]
            {
                $"grabbing {questDefinition.questName}",
                $"picking up {questDefinition.questName}",
                "taking this quest real quick",
                "quest accepted, heading out"
            }
        };
    }

    private string[] GetHuntingMessages()
    {
        return personalityType switch
        {
            FakePlayerPersonalityType.Helpful => new[]
            {
                $"{questDefinition.targetEnemyName}s are up near the hunt area",
                "plenty of mobs here if anyone needs them",
                "careful, a few are close together"
            },

            FakePlayerPersonalityType.Grumpy => new[]
            {
                $"these {questDefinition.targetEnemyName.ToLower()}s are annoying",
                "respawns feel slow",
                "why are they so spread out",
                "this quest is padding lol"
            },

            FakePlayerPersonalityType.Tryhard => new[]
            {
                "chain pulling",
                "clean rotation",
                "optimizing this route",
                "no downtime"
            },

            FakePlayerPersonalityType.Newbie => new[]
            {
                "oh no i pulled another one",
                "am i supposed to loot these?",
                "how do i know when the quest is done?",
                "these are harder than i expected"
            },

            _ => new[]
            {
                $"these {questDefinition.targetEnemyName.ToLower()}s are everywhere",
                "pulling another one",
                "almost done here",
                "this spawn rate is actually decent",
                "anyone else doing this quest?"
            }
        };
    }

    private string[] GetQuestCompleteMessages()
    {
        return personalityType switch
        {
            FakePlayerPersonalityType.Helpful => new[]
            {
                "done here, heading back if anyone needs follow",
                "quest complete, returning to town",
                "finished objective"
            },

            FakePlayerPersonalityType.Grumpy => new[]
            {
                "finally done",
                "about time",
                "done. never loved that quest"
            },

            FakePlayerPersonalityType.Tryhard => new[]
            {
                "objective complete, turn-in next",
                "done. clean run",
                "finished faster than expected"
            },

            FakePlayerPersonalityType.Newbie => new[]
            {
                "it says complete now!",
                "i think i'm done?",
                "where do i turn this in again?"
            },

            _ => new[]
            {
                "done with this one, heading back",
                "quest complete, turning it in",
                "finally done lol",
                "easy enough, going back to town"
            }
        };
    }

    private string[] GetTurnInMessages()
    {
        return personalityType switch
        {
            FakePlayerPersonalityType.Helpful => new[]
            {
                $"turned in {questDefinition.questName}",
                "turn-in done, good luck everyone",
                "quest complete and turned in"
            },

            FakePlayerPersonalityType.Grumpy => new[]
            {
                "turned in. finally",
                "done with that nonsense",
                "quest turned in, moving on"
            },

            FakePlayerPersonalityType.Tryhard => new[]
            {
                "turn-in complete",
                "XP secured",
                "quest cycle done"
            },

            FakePlayerPersonalityType.Newbie => new[]
            {
                "oh nice i turned it in",
                "i found the quest giver!",
                "that worked!"
            },

            _ => new[]
            {
                $"turned in {questDefinition.questName}",
                "nice, free XP",
                "quest done",
                "easy turn-in"
            }
        };
    }

    private string[] GetRestMessages()
    {
        return personalityType switch
        {
            FakePlayerPersonalityType.Helpful => new[]
            {
                "going to help around town for a bit",
                "taking a break, whisper if you need help",
                "back in town now"
            },

            FakePlayerPersonalityType.Grumpy => new[]
            {
                "taking a break from this grind",
                "enough questing for now",
                "wandering until something better shows up"
            },

            FakePlayerPersonalityType.Tryhard => new[]
            {
                "resetting route",
                "checking next objective",
                "downtime phase"
            },

            FakePlayerPersonalityType.Newbie => new[]
            {
                "i'm just gonna wander now",
                "what should i do next?",
                "still learning this zone"
            },

            _ => new[]
            {
                "gonna wander for a bit",
                "taking a break from questing",
                "back to town stuff",
                "might do another quest later"
            }
        };
    }
}