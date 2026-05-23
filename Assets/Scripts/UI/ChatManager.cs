using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI chatText;

    [Header("Settings")]
    [SerializeField] private int maxMessages = 40;

    public static ChatManager Instance { get; private set; }

    private readonly Queue<string> messages = new();

    private void Awake()
    {
        Instance = this;
    }

    public void AddMessage(ChatChannel channel, string sender, string message)
    {
        string formattedMessage = FormatMessage(channel, sender, message);

        messages.Enqueue(formattedMessage);

        while (messages.Count > maxMessages)
        {
            messages.Dequeue();
        }

        RefreshChatText();
    }

    public void AddSystemMessage(string message)
    {
        AddMessage(ChatChannel.System, "System", message);
    }

    private string FormatMessage(ChatChannel channel, string sender, string message)
    {
        string channelText = GetChannelText(channel);
        string channelColor = GetChannelColor(channel);

        if (channel == ChatChannel.System)
        {
            return $"<color={channelColor}>[{channelText}] {message}</color>";
        }

        return $"<color={channelColor}>[{channelText}]</color> <b>{sender}:</b> {message}";
    }

    private string GetChannelText(ChatChannel channel)
    {
        return channel switch
        {
            ChatChannel.System => "System",
            ChatChannel.Global => "Global",
            ChatChannel.Zone => "Zone",
            ChatChannel.Trade => "Trade",
            ChatChannel.Party => "Party",
            ChatChannel.Guild => "Guild",
            ChatChannel.Whisper => "Whisper",
            _ => "Chat"
        };
    }

    private string GetChannelColor(ChatChannel channel)
    {
        return channel switch
        {
            ChatChannel.System => "#FFD45A",
            ChatChannel.Global => "#FFFFFF",
            ChatChannel.Zone => "#9ED8FF",
            ChatChannel.Trade => "#FFB347",
            ChatChannel.Party => "#A58CFF",
            ChatChannel.Guild => "#7CFF7C",
            ChatChannel.Whisper => "#FF8CDB",
            _ => "#FFFFFF"
        };
    }

    private void RefreshChatText()
    {
        if (chatText == null)
        {
            return;
        }

        chatText.text = string.Join("\n", messages);
    }
}