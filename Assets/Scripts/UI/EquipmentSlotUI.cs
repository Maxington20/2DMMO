using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Slot")]
    [SerializeField] private EquipmentSlotType slotType;

    [Header("References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI slotLabelText;

    private ItemDefinition currentItem;

    public EquipmentSlotType SlotType => slotType;

    public void SetItem(ItemDefinition item)
    {
        currentItem = item;

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
        currentItem = null;

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

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentItem == null)
        {
            return;
        }

        ItemTooltipUI.Instance?.Show(currentItem);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ItemTooltipUI.Instance?.Hide();
    }
}