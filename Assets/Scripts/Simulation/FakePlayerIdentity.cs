using UnityEngine;

public class FakePlayerIdentity : MonoBehaviour
{
    [SerializeField] private string displayName = "Fake Player";
    [SerializeField] private FakePlayerPersonalityType personalityType = FakePlayerPersonalityType.Casual;

    public string DisplayName => displayName;
    public FakePlayerPersonalityType PersonalityType => personalityType;

    public void Say(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        ChatManager.Instance?.AddMessage(ChatChannel.Zone, displayName, message);
    }

    public void SayRandom(string[] messages)
    {
        if (messages == null || messages.Length == 0)
        {
            return;
        }

        Say(messages[Random.Range(0, messages.Length)]);
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
                "remember to sell junk before heading out"
            },

            FakePlayerPersonalityType.Grumpy => new[]
            {
                "town is way too crowded",
                "why is everyone standing on the quest giver",
                "auction prices are probably awful again"
            },

            FakePlayerPersonalityType.Tryhard => new[]
            {
                "optimizing route",
                "checking next objective",
                "no downtime"
            },

            FakePlayerPersonalityType.Newbie => new[]
            {
                "where do i train abilities?",
                "is this the starting town?",
                "how do i sell junk?"
            },

            _ => new[]
            {
                "just chilling for a bit",
                "heading out soon",
                "anyone else questing?"
            }
        };
    }
}