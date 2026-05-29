using UnityEngine;

public class FakePlayerIdentity : MonoBehaviour
{
    [Header("Identity")]
    [SerializeField] private string displayName = "Fake Player";
    [SerializeField] private FakePlayerPersonalityType personalityType = FakePlayerPersonalityType.Casual;

    [Header("Guild")]
    [SerializeField] private string guildName = "";
    [SerializeField] private bool showGuildTagInChat = true;

    public string DisplayName => displayName;
    public FakePlayerPersonalityType PersonalityType => personalityType;
    public string GuildName => guildName;
    public bool HasGuild => !string.IsNullOrWhiteSpace(guildName);

    private void Awake()
    {
        SyncCombatEntityName();
    }

    private void OnValidate()
    {
        SyncCombatEntityName();
    }

    public void Say(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        ChatManager.Instance?.AddMessage(ChatChannel.Zone, GetChatDisplayName(), message);
    }

    public void SayToChannel(ChatChannel channel, string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        ChatManager.Instance?.AddMessage(channel, GetChatDisplayName(), message);
    }

    public void SayRandom(string[] messages)
    {
        if (messages == null || messages.Length == 0)
        {
            return;
        }

        Say(messages[Random.Range(0, messages.Length)]);
    }

    public string GetChatDisplayName()
    {
        if (showGuildTagInChat && HasGuild)
        {
            return $"{displayName} <{guildName}>";
        }

        return displayName;
    }

    public string GetProgressMessage(QuestDefinition quest, int currentKills)
    {
        if (quest == null)
        {
            return string.Empty;
        }

        return personalityType switch
        {
            FakePlayerPersonalityType.Helpful =>
                $"{quest.questName}: {currentKills}/{quest.requiredKills} done if anyone is tracking",

            FakePlayerPersonalityType.Grumpy =>
                $"{currentKills}/{quest.requiredKills}. this is taking forever",

            FakePlayerPersonalityType.Tryhard =>
                $"{currentKills}/{quest.requiredKills}. efficient route so far",

            FakePlayerPersonalityType.Newbie =>
                $"i got {currentKills}/{quest.requiredKills}, is that good?",

            _ =>
                $"{quest.questName}: {currentKills}/{quest.requiredKills} {quest.targetEnemyName} defeated."
        };
    }

    public string GetXpMessage(QuestDefinition quest)
    {
        if (quest == null)
        {
            return string.Empty;
        }

        return personalityType switch
        {
            FakePlayerPersonalityType.Helpful => $"got {quest.xpReward} XP from that quest",
            FakePlayerPersonalityType.Grumpy => $"only {quest.xpReward} XP? alright whatever",
            FakePlayerPersonalityType.Tryhard => $"+{quest.xpReward} XP. moving on",
            FakePlayerPersonalityType.Newbie => $"wait nice, i got {quest.xpReward} XP",
            _ => $"gained {quest.xpReward} XP."
        };
    }

    public string[] GetIdleMessages()
    {
        return personalityType switch
        {
            FakePlayerPersonalityType.Helpful => new[]
            {
                "anyone need help with quests?",
                "i can help if someone needs a hand",
                "remember to sell junk before heading out",
                HasGuild ? $"{guildName} is helping around town if anyone needs a hand" : "happy to help if anyone is stuck"
            },

            FakePlayerPersonalityType.Grumpy => new[]
            {
                "town is way too crowded",
                "why is everyone standing on the quest giver",
                "auction prices are probably awful again",
                HasGuild ? $"half of {guildName} is probably afk again" : "everyone is afk again"
            },

            FakePlayerPersonalityType.Tryhard => new[]
            {
                "optimizing route",
                "checking next objective",
                "no downtime",
                HasGuild ? $"{guildName} route is faster than this public one" : "route could be cleaner"
            },

            FakePlayerPersonalityType.Newbie => new[]
            {
                "where do i train abilities?",
                "is this the starting town?",
                "how do i sell junk?",
                HasGuild ? $"someone in {guildName} told me to come here" : "am i supposed to be here?"
            },

            _ => new[]
            {
                "just chilling for a bit",
                "heading out soon",
                "anyone else questing?",
                HasGuild ? $"waiting on someone from {guildName}" : "might join a group soon"
            }
        };
    }

    private void SyncCombatEntityName()
    {
        CombatEntity combatEntity = GetComponent<CombatEntity>();

        if (combatEntity != null)
        {
            combatEntity.SetDisplayName(displayName);
        }
    }
}