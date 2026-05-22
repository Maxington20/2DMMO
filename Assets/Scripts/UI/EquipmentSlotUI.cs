using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlotUI : MonoBehaviour
{
    [Header("Slot")]
    [SerializeField] private EquipmentSlotType slotType;

    [Header("References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI slotLabelText;

    public EquipmentSlotType SlotType => slotType;

    public void SetItem(ItemDefinition item)
    {
        if (item == null)
        {
            Clear();
            return;
        }

        if (iconImage != null)
        {
            iconImage.gameObject.SetActive(true);
            iconImage.sprite = item.icon;
        }
    }

    public void Clear()
    {
        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.gameObject.SetActive(false);
        }
    }

    public void SetLabel(string label)
    {
        if (slotLabelText != null)
        {
            slotLabelText.text = label;
        }
    }
}