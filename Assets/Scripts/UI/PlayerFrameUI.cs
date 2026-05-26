using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFrameUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health playerHealth;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private Image healthBarFill;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Display")]
    [SerializeField] private string playerName = "Player";

    private void Start()
    {
        RefreshName();
    }

    private void Update()
    {
        if (playerHealth == null)
        {
            return;
        }

        float healthPercent = (float)playerHealth.CurrentHealth / playerHealth.MaxHealth;

        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = healthPercent;
        }

        if (healthText != null)
        {
            healthText.text = $"{playerHealth.CurrentHealth} / {playerHealth.MaxHealth}";
        }
    }

    public void SetPlayerName(string newName)
    {
        playerName = newName;
        RefreshName();
    }

    private void RefreshName()
    {
        if (playerNameText != null)
        {
            playerNameText.text = playerName;
        }
    }
}