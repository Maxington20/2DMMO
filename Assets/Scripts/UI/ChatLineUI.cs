using TMPro;
using UnityEngine;

public class ChatLineUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lineText;

    public void SetText(string message)
    {
        if (lineText != null)
        {
            lineText.text = message;
        }
    }
}