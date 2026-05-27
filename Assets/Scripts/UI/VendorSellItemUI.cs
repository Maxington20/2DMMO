using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VendorSellItemUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private Button sellButton;

    private InventoryItem inventoryItem;
    private VendorWindowUI vendorWindow;

    private void Awake()
    {
        if (sellButton != null)
        {
            sellButton.onClick.AddListener(SellItem);
        }
    }

    public void Initialize(InventoryItem item, VendorWindowUI window)
    {
        inventoryItem = item;
        vendorWindow = window;

        if (inventoryItem == null || inventoryItem.ItemDefinition == null)
        {
            return;
        }

        ItemDefinition itemDefinition = inventoryItem.ItemDefinition;

        if (iconImage != null)
        {
            iconImage.sprite = itemDefinition.icon;
            iconImage.gameObject.SetActive(itemDefinition.icon != null);
        }

        if (itemNameText != null)
        {
            string quantityText = inventoryItem.Quantity > 1
                ? $" x{inventoryItem.Quantity}"
                : string.Empty;

            itemNameText.text = $"{itemDefinition.itemName}{quantityText}";
        }

        if (valueText != null)
        {
            valueText.text = $"{itemDefinition.sellValue} copper";
        }
    }

    private void SellItem()
    {
        if (inventoryItem == null || inventoryItem.ItemDefinition == null || vendorWindow == null)
        {
            return;
        }

        vendorWindow.SellItem(inventoryItem.ItemDefinition);
    }
}