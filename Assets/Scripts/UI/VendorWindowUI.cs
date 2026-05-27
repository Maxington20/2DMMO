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

    [Header("Sell Items")]
    [SerializeField] private RectTransform sellItemsParent;
    [SerializeField] private VendorSellItemUI sellItemPrefab;

    [Header("Row Layout")]
    [SerializeField] private float rowHeight = 54f;
    [SerializeField] private float rowSpacing = 6f;
    [SerializeField] private float topPadding = 12f;

    private PlayerInventory playerInventory;
    private PlayerCurrency playerCurrency;

    private readonly List<VendorSellItemUI> spawnedRows = new();

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
            playerInventory.OnInventoryChanged -= RefreshSellItems;
        }
    }

    public void Show(VendorNPC vendor)
    {
        if (vendor == null)
        {
            return;
        }

        FindPlayerReferences();

        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged -= RefreshSellItems;
            playerInventory.OnInventoryChanged += RefreshSellItems;
        }

        root.SetActive(true);
        GameplayInputLock.SetLocked(this, true);

        if (vendorNameText != null)
        {
            vendorNameText.text = vendor.VendorName;
        }

        RefreshSellItems();
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
            playerInventory.OnInventoryChanged -= RefreshSellItems;
        }
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

        RefreshSellItems();
    }

    private void RefreshSellItems()
    {
        ClearRows();

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

            RectTransform rowRect = row.GetComponent<RectTransform>();

            rowRect.anchorMin = new Vector2(0f, 1f);
            rowRect.anchorMax = new Vector2(1f, 1f);
            rowRect.pivot = new Vector2(0.5f, 1f);

            rowRect.offsetMin = new Vector2(12f, rowRect.offsetMin.y);
            rowRect.offsetMax = new Vector2(-12f, rowRect.offsetMax.y);

            float y = -topPadding - rowIndex * (rowHeight + rowSpacing);

            rowRect.anchoredPosition = new Vector2(0f, y);
            rowRect.sizeDelta = new Vector2(rowRect.sizeDelta.x, rowHeight);

            spawnedRows.Add(row);
            rowIndex++;
        }
    }

    private void ClearRows()
    {
        foreach (VendorSellItemUI row in spawnedRows)
        {
            if (row != null)
            {
                Destroy(row.gameObject);
            }
        }

        spawnedRows.Clear();
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