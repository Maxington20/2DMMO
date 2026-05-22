using UnityEngine;
using UnityEngine.InputSystem;

public class EquipmentUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject equipmentPanel;
    [SerializeField] private PlayerEquipment playerEquipment;
    [SerializeField] private EquipmentSlotUI[] equipmentSlots;

    private void Start()
    {
        if (equipmentPanel != null)
        {
            equipmentPanel.SetActive(false);
        }

        if (playerEquipment != null)
        {
            playerEquipment.OnEquipmentChanged += Refresh;
        }

        Refresh();
    }

    private void OnDestroy()
    {
        if (playerEquipment != null)
        {
            playerEquipment.OnEquipmentChanged -= Refresh;
        }
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard != null && keyboard.cKey.wasPressedThisFrame)
        {
            ToggleEquipment();
        }
    }

    private void ToggleEquipment()
    {
        if (equipmentPanel == null)
        {
            return;
        }

        equipmentPanel.SetActive(!equipmentPanel.activeSelf);

        if (equipmentPanel.activeSelf)
        {
            Refresh();
        }
    }

    private void Refresh()
    {
        if (playerEquipment == null || equipmentSlots == null)
        {
            return;
        }

        foreach (EquipmentSlotUI slot in equipmentSlots)
        {
            if (slot == null)
            {
                continue;
            }

            ItemDefinition equippedItem =
                playerEquipment.GetEquippedItem(slot.SlotType);

            slot.SetItem(equippedItem);
        }
    }
}