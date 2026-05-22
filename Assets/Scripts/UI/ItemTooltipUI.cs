using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemTooltipUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI rarityText;
    [SerializeField] private TextMeshProUGUI sellValueText;

    [Header("Position")]
    [SerializeField] private Vector2 screenOffset = new Vector2(32f, -32f);

    public static ItemTooltipUI Instance { get; private set; }

    private RectTransform rectTransform;
    private bool isVisible;

    private void Awake()
    {
        Instance = this;
        rectTransform = GetComponent<RectTransform>();

        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        Hide();
    }

    private void Update()
    {
        if (!isVisible)
        {
            return;
        }

        Mouse mouse = Mouse.current;

        if (mouse == null)
        {
            return;
        }

        rectTransform.position = mouse.position.ReadValue() + screenOffset;
    }

    public void Show(ItemDefinition item)
    {
        if (item == null)
        {
            Hide();
            return;
        }

        isVisible = true;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        if (nameText != null)
        {
            nameText.text = item.itemName;
            nameText.color = GetRarityColor(item.rarity);
        }

        if (rarityText != null)
        {
            rarityText.text = BuildRarityAndTypeText(item);
            rarityText.color = GetRarityColor(item.rarity);
        }

        if (sellValueText != null)
        {
            sellValueText.text = BuildDetailText(item);
        }
    }

    public void Hide()
    {
        isVisible = false;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    private string BuildRarityAndTypeText(ItemDefinition item)
    {
        if (item.itemType == ItemType.Equipment)
        {
            return $"{item.rarity} {item.equipmentSlot}";
        }

        return item.rarity.ToString();
    }

    private string BuildDetailText(ItemDefinition item)
    {
        string details = string.Empty;

        if (item.itemType == ItemType.Equipment)
        {
            if (item.bonusDamage > 0)
            {
                details += $"+{item.bonusDamage} Damage\n";
            }

            if (item.bonusMaxHealth > 0)
            {
                details += $"+{item.bonusMaxHealth} Max Health\n";
            }
        }

        details += $"Sell Value: {item.sellValue} copper";

        return details;
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