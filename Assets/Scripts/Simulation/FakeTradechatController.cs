using UnityEngine;

[RequireComponent(typeof(FakePlayerIdentity))]
public class FakeTradeChatController : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float minDelaySeconds = 45f;
    [SerializeField] private float maxDelaySeconds = 120f;

    [Header("Behavior")]
    [SerializeField] private bool onlyChatWhenIdle = true;

    private FakePlayerIdentity identity;
    private FakePlayerQuestController questController;
    private float timer;

    private void Awake()
    {
        identity = GetComponent<FakePlayerIdentity>();
        questController = GetComponent<FakePlayerQuestController>();
    }

    private void Start()
    {
        ResetTimer();
    }

    private void Update()
    {
        if (identity == null)
        {
            return;
        }

        if (onlyChatWhenIdle && questController != null && questController.enabled)
        {
            return;
        }

        timer -= Time.deltaTime;

        if (timer > 0f)
        {
            return;
        }

        SayRandomTradeMessage();
        ResetTimer();
    }

    private void SayRandomTradeMessage()
    {
        string[] messages = GetTradeMessages();

        if (messages == null || messages.Length == 0)
        {
            return;
        }

        string message = messages[Random.Range(0, messages.Length)];

        identity.SayToChannel(ChatChannel.Trade, message);
    }

    private string[] GetTradeMessages()
    {
        string guildName = identity.GuildName;

        return identity.PersonalityType switch
        {
            FakePlayerPersonalityType.Helpful => new[]
            {
                "WTS extra minor health potions cheap",
                "anyone need help finding the vendor?",
                "selling spare wolf pelts if anyone needs crafting mats later",
                "remember to vendor junk before heading back out",
                identity.HasGuild ? $"{guildName} has extra potions if anyone needs" : "extra potions available"
            },

            FakePlayerPersonalityType.Grumpy => new[]
            {
                "WTS torn cloth, vendor trash basically",
                "prices in this town are awful",
                "anyone buying stolen trinkets or am i just vendoring these",
                "trade chat is dead again",
                identity.HasGuild ? $"anyone from {guildName} actually online?" : "anyone actually online?"
            },

            FakePlayerPersonalityType.Tryhard => new[]
            {
                "WTB potions in bulk",
                "WTS bandit loot, fast sale only",
                "buying consumables before next route",
                "need cheap potions, paying copper",
                identity.HasGuild ? $"{guildName} buying consumables for route prep" : "buying consumables for route prep"
            },

            FakePlayerPersonalityType.Newbie => new[]
            {
                "how do i sell items?",
                "is bent dagger worth keeping?",
                "does anyone buy torn cloth?",
                "where is the merchant?",
                identity.HasGuild ? $"someone from {guildName} said to ask in trade?" : "am i using trade chat right?"
            },

            _ => new[]
            {
                "WTS wolf pelts",
                "WTB minor health potions",
                "selling junk near vendor",
                "anyone need bandit drops?",
                identity.HasGuild ? $"{guildName} looking for cheap potions" : "looking for cheap potions"
            }
        };
    }

    private void ResetTimer()
    {
        timer = Random.Range(minDelaySeconds, maxDelaySeconds);
    }
}