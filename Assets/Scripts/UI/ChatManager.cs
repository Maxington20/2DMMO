using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private ScrollRect chatScrollRect;
    [SerializeField] private Transform contentParent;
    [SerializeField] private ChatLineUI chatLinePrefab;

    [Header("Settings")]
    [SerializeField] private int maxMessages = 80;
    [SerializeField] private float bottomThreshold = 0.05f;

    public static ChatManager Instance { get; private set; }

    private readonly Queue<ChatLineUI> spawnedLines = new();

    private void Awake()
    {
        Instance = this;
    }

    public void AddMessage(ChatChannel channel, string sender, string message)
    {
        bool shouldAutoScroll = IsAtBottom();

        string formattedMessage = FormatMessage(channel, sender, message);

        ChatLineUI line = Instantiate(chatLinePrefab, contentParent);
        line.SetText(formattedMessage);

        spawnedLines.Enqueue(line);

        while (spawnedLines.Count > maxMessages)
        {
            ChatLineUI oldLine = spawnedLines.Dequeue();

            if (oldLine != null)
            {
                Destroy(oldLine.gameObject);
            }
        }

        if (shouldAutoScroll)
        {
            StartCoroutine(ScrollToBottomNextFrame());
        }
    }

    public void AddSystemMessage(string message)
    {
        AddMessage(ChatChannel.System, "System", message);
    }

    private bool IsAtBottom()
    {
        if (chatScrollRect == null)
        {
            return true;
        }

        return chatScrollRect.verticalNormalizedPosition <= bottomThreshold;
    }

    private IEnumerator ScrollToBottomNextFrame()
    {
        yield return null;
        Canvas.ForceUpdateCanvases();

        if (chatScrollRect != null)
        {
            chatScrollRect.verticalNormalizedPosition = 0f;
        }

        Canvas.ForceUpdateCanvases();
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
}