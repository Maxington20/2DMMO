using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VendorBuyItemUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Button buyButton;

    private VendorStockItem stockItem;
    private VendorWindowUI vendorWindow;

    private void Awake()
    {
        if (buyButton != null)
        {
            buyButton.onClick.AddListener(BuyItem);
        }
    }

    public void Initialize(VendorStockItem item, VendorWindowUI window)
    {
        stockItem = item;
        vendorWindow = window;

        if (stockItem == null || stockItem.item == null)
        {
            return;
        }

        if (iconImage != null)
        {
            iconImage.sprite = stockItem.item.icon;
            iconImage.gameObject.SetActive(stockItem.item.icon != null);
        }

        if (itemNameText != null)
        {
            itemNameText.text = stockItem.item.itemName;
        }

        if (priceText != null)
        {
            priceText.text = $"{stockItem.priceCopper} copper";
        }
    }

    private void BuyItem()
    {
        if (stockItem == null || vendorWindow == null)
        {
            return;
        }

        vendorWindow.BuyItem(stockItem);
    }
}