using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    [Header("Items")]
    [SerializeField] private ItemDefinition[] items;

    public static ItemDatabase Instance { get; private set; }

    private readonly Dictionary<string, ItemDefinition> itemsById = new();

    private void Awake()
    {
        Instance = this;
        BuildLookup();
    }

    private void BuildLookup()
    {
        itemsById.Clear();

        foreach (ItemDefinition item in items)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.itemId))
            {
                continue;
            }

            if (itemsById.ContainsKey(item.itemId))
            {
                Debug.LogWarning($"Duplicate item ID found: {item.itemId}");
                continue;
            }

            itemsById.Add(item.itemId, item);
        }
    }

    public ItemDefinition GetItemById(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            return null;
        }

        itemsById.TryGetValue(itemId, out ItemDefinition item);
        return item;
    }
}