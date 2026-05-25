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
            GameplayInputLock.SetLocked(this, false);
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

        GameplayInputLock.ClearLock(this);
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

        bool newState = !equipmentPanel.activeSelf;

        equipmentPanel.SetActive(newState);
        GameplayInputLock.SetLocked(this, newState);

        if (newState)
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