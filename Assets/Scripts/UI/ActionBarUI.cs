using TMPro;
using UnityEngine;

public class ActionBarUI : MonoBehaviour
{
    [Header("Slot 1")]
    [SerializeField] private TextMeshProUGUI slotOneLabelText;

    public void SetSlotOneAbilityName(string abilityName)
    {
        if (slotOneLabelText != null)
        {
            slotOneLabelText.text = abilityName;
        }
    }
}