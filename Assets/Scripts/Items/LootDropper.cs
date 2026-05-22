using UnityEngine;

public class LootDropper : MonoBehaviour
{
    [Header("Loot")]
    [SerializeField] private LootTableEntry[] lootTable;

    [Header("Pickup Prefab")]
    [SerializeField] private LootPickup lootPickupPrefab;

    public void DropLoot()
    {
        if (lootPickupPrefab == null || lootTable == null)
        {
            return;
        }

        foreach (LootTableEntry entry in lootTable)
        {
            if (entry == null || entry.item == null)
            {
                continue;
            }

            float roll = Random.value;

            if (roll <= entry.dropChance)
            {
                Vector3 dropPosition = transform.position;
                dropPosition += new Vector3(
                    Random.Range(-0.35f, 0.35f),
                    Random.Range(-0.35f, 0.35f),
                    0f
                );

                LootPickup pickup = Instantiate(
                    lootPickupPrefab,
                    dropPosition,
                    Quaternion.identity
                );

                pickup.Initialize(entry.item);
            }
        }
    }
}