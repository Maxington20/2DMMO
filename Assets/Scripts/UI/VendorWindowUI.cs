using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VendorWindowUI : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject root;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI vendorNameText;

    [Header("Buttons")]
    [SerializeField] private Button closeButton;

    [Header("Buy Items")]
    [SerializeField] private RectTransform buyItemsParent;
    [SerializeField] private VendorBuyItemUI buyItemPrefab;

    [Header("Sell Items")]
    [SerializeField] private RectTransform sellItemsParent;
    [SerializeField] private VendorSellItemUI sellItemPrefab;

    [Header("Row Layout")]
    [SerializeField] private float rowHeight = 54f;
    [SerializeField] private float rowSpacing = 6f;
    [SerializeField] private float topPadding = 8f;

    private VendorNPC currentVendor;
    private PlayerInventory playerInventory;
    private PlayerCurrency playerCurrency;

    private readonly List<VendorBuyItemUI> spawnedBuyRows = new();
    private readonly List<VendorSellItemUI> spawnedSellRows = new();

    public static VendorWindowUI Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Hide);
        }

        Hide();
    }

    private void OnDestroy()
    {
        GameplayInputLock.ClearLock(this);

        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged -= RefreshAllItems;
        }
    }

    public void Show(VendorNPC vendor)
    {
        if (vendor == null)
        {
            return;
        }

        currentVendor = vendor;

        FindPlayerReferences();

        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged -= RefreshAllItems;
            playerInventory.OnInventoryChanged += RefreshAllItems;
        }

        root.SetActive(true);
        GameplayInputLock.SetLocked(this, true);

        if (vendorNameText != null)
        {
            vendorNameText.text = vendor.VendorName;
        }

        RefreshAllItems();
    }

    public void Hide()
    {
        if (root != null)
        {
            root.SetActive(false);
        }

        GameplayInputLock.SetLocked(this, false);

        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged -= RefreshAllItems;
        }

        currentVendor = null;
    }

    public void BuyItem(VendorStockItem stockItem)
    {
        if (stockItem == null || stockItem.item == null || playerInventory == null || playerCurrency == null)
        {
            return;
        }

        if (playerCurrency.Copper < stockItem.priceCopper)
        {
            ChatManager.Instance?.AddSystemMessage("You do not have enough copper.");
            return;
        }

        if (!playerInventory.HasSpaceFor(stockItem.item))
        {
            ChatManager.Instance?.AddSystemMessage("Your inventory is full.");
            return;
        }

        bool paid = playerCurrency.RemoveCopper(stockItem.priceCopper);

        if (!paid)
        {
            return;
        }

        bool added = playerInventory.AddItem(stockItem.item, 1);

        if (!added)
        {
            playerCurrency.AddCopper(stockItem.priceCopper);
            return;
        }

        ChatManager.Instance?.AddSystemMessage(
            $"Bought {stockItem.item.itemName} for {stockItem.priceCopper} copper."
        );

        RefreshAllItems();
    }

    public void SellItem(ItemDefinition item)
    {
        if (item == null || playerInventory == null || playerCurrency == null)
        {
            return;
        }

        bool removed = playerInventory.RemoveItem(item, 1);

        if (!removed)
        {
            return;
        }

        playerCurrency.AddCopper(item.sellValue);

        ChatManager.Instance?.AddSystemMessage(
            $"Sold {item.itemName} for {item.sellValue} copper."
        );

        RefreshAllItems();
    }

    private void RefreshAllItems()
    {
        RefreshBuyItems();
        RefreshSellItems();
    }

    private void RefreshBuyItems()
    {
        ClearBuyRows();

        if (currentVendor == null || buyItemsParent == null || buyItemPrefab == null)
        {
            return;
        }

        VendorStockItem[] stock = currentVendor.StockItems;

        if (stock == null)
        {
            return;
        }

        int rowIndex = 0;

        foreach (VendorStockItem stockItem in stock)
        {
            if (stockItem == null || stockItem.item == null)
            {
                continue;
            }

            VendorBuyItemUI row = Instantiate(buyItemPrefab, buyItemsParent);
            row.Initialize(stockItem, this);

            PositionRow(row.GetComponent<RectTransform>(), rowIndex);

            spawnedBuyRows.Add(row);
            rowIndex++;
        }
    }

    private void RefreshSellItems()
    {
        ClearSellRows();

        if (playerInventory == null || sellItemsParent == null || sellItemPrefab == null)
        {
            return;
        }

        int rowIndex = 0;

        foreach (InventoryItem item in playerInventory.Items)
        {
            if (item == null || item.ItemDefinition == null)
            {
                continue;
            }

            VendorSellItemUI row = Instantiate(sellItemPrefab, sellItemsParent);
            row.Initialize(item, this);

            PositionRow(row.GetComponent<RectTransform>(), rowIndex);

            spawnedSellRows.Add(row);
            rowIndex++;
        }
    }

    private void PositionRow(RectTransform rowRect, int rowIndex)
    {
        if (rowRect == null)
        {
            return;
        }

        rowRect.anchorMin = new Vector2(0f, 1f);
        rowRect.anchorMax = new Vector2(1f, 1f);
        rowRect.pivot = new Vector2(0.5f, 1f);

        rowRect.offsetMin = new Vector2(12f, rowRect.offsetMin.y);
        rowRect.offsetMax = new Vector2(-12f, rowRect.offsetMax.y);

        float y = -topPadding - rowIndex * (rowHeight + rowSpacing);

        rowRect.anchoredPosition = new Vector2(0f, y);
        rowRect.sizeDelta = new Vector2(rowRect.sizeDelta.x, rowHeight);
    }

    private void ClearBuyRows()
    {
        foreach (VendorBuyItemUI row in spawnedBuyRows)
        {
            if (row != null)
            {
                Destroy(row.gameObject);
            }
        }

        spawnedBuyRows.Clear();
    }

    private void ClearSellRows()
    {
        foreach (VendorSellItemUI row in spawnedSellRows)
        {
            if (row != null)
            {
                Destroy(row.gameObject);
            }
        }

        spawnedSellRows.Clear();
    }

    private void FindPlayerReferences()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            return;
        }

        playerInventory = playerObject.GetComponent<PlayerInventory>();
        playerCurrency = playerObject.GetComponent<PlayerCurrency>();
    }
}