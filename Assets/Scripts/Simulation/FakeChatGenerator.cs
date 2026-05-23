using UnityEngine;

public class FakeChatGenerator : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float minMessageDelay = 3f;
    [SerializeField] private float maxMessageDelay = 8f;

    private float messageTimer;

    private readonly string[] fakePlayerNames =
    {
        "Thornspark",
        "CopperKeg",
        "Mira",
        "Velmora",
        "Rustpike",
        "Nyxen",
        "Bramble",
        "Ketta",
        "Ashwyn",
        "Gobwick"
    };

    private readonly string[] zoneMessages =
    {
        "anyone seen the alpha wolf?",
        "where do cracked fangs drop?",
        "wolves are packed right now lol",
        "need one more for ruins when people are ready",
        "bandits hit harder than I expected",
        "is the bear cave soloable?",
        "anyone else getting wrecked by gnolls?",
        "starter zone is actually pretty nice"
    };

    private readonly string[] globalMessages =
    {
        "what class is everyone playing?",
        "priest seems really strong early",
        "warrior feels slow but tanky",
        "wizard damage is nuts if you don't get hit",
        "any guilds recruiting new players?",
        "this game is way prettier than I expected",
        "artificer better not be overpowered again",
        "ranger pets when?"
    };

    private readonly string[] tradeMessages =
    {
        "WTS wolf pelts cheap",
        "buying cracked fangs",
        "anyone selling rusty sword?",
        "WTS low level mats",
        "LF crafter for starter gear",
        "selling bear claws pst",
        "buying iron scraps",
        "cheap pelts by the gate"
    };

    private void Start()
    {
        ResetTimer();

        ChatManager.Instance?.AddSystemMessage("Connected to Arcfall Online.");
        ChatManager.Instance?.AddMessage(ChatChannel.Zone, "Thornspark", "anyone else just starting?");
    }

    private void Update()
    {
        messageTimer -= Time.deltaTime;

        if (messageTimer <= 0f)
        {
            GenerateRandomMessage();
            ResetTimer();
        }
    }

    private void GenerateRandomMessage()
    {
        ChatChannel channel = PickRandomChannel();
        string sender = PickRandom(fakePlayerNames);
        string message = channel switch
        {
            ChatChannel.Trade => PickRandom(tradeMessages),
            ChatChannel.Global => PickRandom(globalMessages),
            _ => PickRandom(zoneMessages)
        };

        ChatManager.Instance?.AddMessage(channel, sender, message);
    }

    private ChatChannel PickRandomChannel()
    {
        int roll = Random.Range(0, 100);

        if (roll < 55)
        {
            return ChatChannel.Zone;
        }

        if (roll < 85)
        {
            return ChatChannel.Global;
        }

        return ChatChannel.Trade;
    }

    private string PickRandom(string[] values)
    {
        return values[Random.Range(0, values.Length)];
    }

    private void ResetTimer()
    {
        messageTimer = Random.Range(minMessageDelay, maxMessageDelay);
    }
}