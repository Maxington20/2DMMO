using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionBarUI : MonoBehaviour
{
    [Header("Controllers")]
    [SerializeField] private PlayerAbilityController abilityController;
    [SerializeField] private PlayerConsumableController consumableController;

    [Header("Slot 1")]
    [SerializeField] private TextMeshProUGUI slotOneLabelText;
    [SerializeField] private Image slotOneCooldownOverlay;
    [SerializeField] private TextMeshProUGUI slotOneCooldownText;

    [Header("Slot 2")]
    [SerializeField] private TextMeshProUGUI slotTwoLabelText;
    [SerializeField] private Image slotTwoCooldownOverlay;
    [SerializeField] private TextMeshProUGUI slotTwoCooldownText;

    private void Update()
    {
        RefreshAbilityCooldown();
        RefreshPotionCooldown();
    }

    public void SetSlotOneAbilityName(string abilityName)
    {
        slotOneLabelText.text = abilityName;
    }

    private void RefreshAbilityCooldown()
    {
        UpdateCooldown(
            abilityController?.PrimaryAbilityCooldownTimer ?? 0,
            abilityController?.PrimaryAbilityCooldownSeconds ?? 0,
            slotOneCooldownOverlay,
            slotOneCooldownText
        );
    }

    private void RefreshPotionCooldown()
    {
        if (slotTwoLabelText != null)
        {
            slotTwoLabelText.text = "Potion";
        }

        UpdateCooldown(
            consumableController?.PotionCooldownTimer ?? 0,
            consumableController?.PotionCooldownSeconds ?? 0,
            slotTwoCooldownOverlay,
            slotTwoCooldownText
        );
    }

    private void UpdateCooldown(
        float timer,
        float maxTime,
        Image overlay,
        TextMeshProUGUI text)
    {
        bool active = timer > 0;

        if (overlay != null)
        {
            overlay.gameObject.SetActive(active);

            if (active)
            {
                overlay.fillAmount = timer / maxTime;
            }
        }

        if (text != null)
        {
            text.gameObject.SetActive(active);

            if (active)
            {
                text.text = Mathf.CeilToInt(timer).ToString();
            }
        }
    }
}