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
        if (playerNameText != null)
        {
            playerNameText.text = playerName;
        }
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
}