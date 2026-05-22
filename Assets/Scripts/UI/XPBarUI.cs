using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class XPBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerProgression progression;
    [SerializeField] private Image xpFill;
    [SerializeField] private TextMeshProUGUI xpText;

    private void Update()
    {
        if (progression == null)
        {
            return;
        }

        float xpPercent = (float)progression.CurrentXp / progression.XpToNextLevel;

        if (xpFill != null)
        {
            xpFill.fillAmount = xpPercent;
        }

        if (xpText != null)
        {
            xpText.text = $"Level {progression.CurrentLevel} — {progression.CurrentXp} / {progression.XpToNextLevel} XP";
        }
    }
}