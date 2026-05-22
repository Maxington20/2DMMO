using TMPro;
using UnityEngine;

public class ItemTooltipUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject tooltipRoot;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI rarityText;
    [SerializeField] private TextMeshProUGUI sellValueText;

    [Header("Position")]
    [SerializeField] private Vector2 screenOffset = new Vector2(18f, -18f);

    public static ItemTooltipUI Instance { get; private set; }

    private RectTransform rectTransform;

    private void Awake()
    {
        Instance = this;
        rectTransform = GetComponent<RectTransform>();

        Hide();
    }

    private void Update()
    {
        if (tooltipRoot == null || !tooltipRoot.activeSelf)
        {
            return;
        }

        rectTransform.position = (Vector2)Input.mousePosition + screenOffset;
    }

    public void Show(ItemDefinition item)
    {
        if (item == null)
        {
            Hide();
            return;
        }

        if (tooltipRoot != null)
        {
            tooltipRoot.SetActive(true);
        }

        if (nameText != null)
        {
            nameText.text = item.itemName;
            nameText.color = GetRarityColor(item.rarity);
        }

        if (rarityText != null)
        {
            rarityText.text = item.rarity.ToString();
            rarityText.color = GetRarityColor(item.rarity);
        }

        if (sellValueText != null)
        {
            sellValueText.text = $"Sell Value: {item.sellValue} copper";
        }
    }

    public void Hide()
    {
        if (tooltipRoot != null)
        {
            tooltipRoot.SetActive(false);
        }
    }

    private Color GetRarityColor(ItemRarity rarity)
    {
        return rarity switch
        {
            ItemRarity.Poor => new Color32(150, 150, 150, 255),
            ItemRarity.Common => new Color32(255, 255, 255, 255),
            ItemRarity.Uncommon => new Color32(30, 255, 80, 255),
            ItemRarity.Rare => new Color32(70, 130, 255, 255),
            ItemRarity.Epic => new Color32(180, 80, 255, 255),
            ItemRarity.Legendary => new Color32(255, 150, 40, 255),
            _ => Color.white
        };
    }
}