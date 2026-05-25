using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private Transform slotGrid;
    [SerializeField] private InventorySlotUI slotPrefab;

    private readonly List<InventorySlotUI> slots = new();

    private void Start()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
            GameplayInputLock.SetLocked(this, false);
        }

        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged += Refresh;
        }

        BuildSlots();
        Refresh();
    }

    private void OnDestroy()
    {
        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged -= Refresh;
        }

        GameplayInputLock.ClearLock(this);
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard != null && keyboard.bKey.wasPressedThisFrame)
        {
            ToggleInventory();
        }
    }

    private void ToggleInventory()
    {
        if (inventoryPanel == null)
        {
            return;
        }

        bool newState = !inventoryPanel.activeSelf;

        inventoryPanel.SetActive(newState);
        GameplayInputLock.SetLocked(this, newState);

        if (newState)
        {
            Refresh();
        }
    }

    private void BuildSlots()
    {
        if (playerInventory == null || slotGrid == null || slotPrefab == null)
        {
            return;
        }

        foreach (Transform child in slotGrid)
        {
            Destroy(child.gameObject);
        }

        slots.Clear();

        for (int i = 0; i < playerInventory.MaxSlots; i++)
        {
            InventorySlotUI slot = Instantiate(slotPrefab, slotGrid);
            slot.Clear();
            slots.Add(slot);
        }
    }

    private void Refresh()
    {
        if (playerInventory == null)
        {
            return;
        }

        for (int i = 0; i < slots.Count; i++)
        {
            if (i < playerInventory.Items.Count)
            {
                slots[i].SetItem(playerInventory.Items[i]);
            }
            else
            {
                slots[i].Clear();
            }
        }
    }
}