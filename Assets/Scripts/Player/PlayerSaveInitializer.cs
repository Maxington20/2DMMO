using UnityEngine;

public class PlayerSaveInitializer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private PlayerEquipment playerEquipment;

    private void Start()
    {
        if (playerInventory != null)
        {
            playerInventory.LoadInventoryFromSave();
        }

        if (playerEquipment != null)
        {
            playerEquipment.LoadEquipmentFromSave();
        }
    }
}