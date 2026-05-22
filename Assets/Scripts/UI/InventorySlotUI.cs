using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI quantityText;

    public void SetItem(InventoryItem inventoryItem)
    {
        if (inventoryItem == null || inventoryItem.ItemDefinition == null)
        {
            Clear();
            return;
        }

        if (iconImage != null)
        {
            iconImage.gameObject.SetActive(true);
            iconImage.sprite = inventoryItem.ItemDefinition.icon;
        }

        if (quantityText != null)
        {
            quantityText.text = inventoryItem.Quantity > 1
                ? inventoryItem.Quantity.ToString()
                : string.Empty;
        }
    }

    public void Clear()
    {
        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.gameObject.SetActive(false);
        }

        if (quantityText != null)
        {
            quantityText.text = string.Empty;
        }
    }
}