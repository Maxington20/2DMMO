using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI quantityText;

    private InventoryItem currentItem;

    public void SetItem(InventoryItem inventoryItem)
    {
        currentItem = inventoryItem;

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
        currentItem = null;

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

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentItem == null || currentItem.ItemDefinition == null)
        {
            return;
        }

        ItemTooltipUI.Instance?.Show(currentItem.ItemDefinition);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ItemTooltipUI.Instance?.Hide();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentItem == null || currentItem.ItemDefinition == null)
        {
            return;
        }

        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            return;
        }

        PlayerEquipment equipment = playerObject.GetComponent<PlayerEquipment>();

        if (equipment == null)
        {
            return;
        }

        equipment.Equip(currentItem.ItemDefinition);
    }
}