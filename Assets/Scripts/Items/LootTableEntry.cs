using System;
using UnityEngine;

[Serializable]
public class LootTableEntry
{
    public ItemDefinition item;
    [Range(0f, 1f)] public float dropChance = 1f;
}