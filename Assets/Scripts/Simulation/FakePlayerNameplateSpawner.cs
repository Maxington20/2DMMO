using System.Collections.Generic;
using UnityEngine;

public class FakePlayerNameplateSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NameplateController nameplatePrefab;
    [SerializeField] private Canvas worldCanvas;

    private readonly List<NameplateController> spawnedNameplates = new();

    private void Start()
    {
        SpawnNameplates();
    }

    private void SpawnNameplates()
    {
        if (nameplatePrefab == null || worldCanvas == null)
        {
            return;
        }

        FakePlayerIdentity[] fakePlayers = FindObjectsOfType<FakePlayerIdentity>();

        foreach (FakePlayerIdentity fakePlayer in fakePlayers)
        {
            if (fakePlayer == null)
            {
                continue;
            }

            NameplateController nameplate = Instantiate(nameplatePrefab, worldCanvas.transform);
            nameplate.Initialize(fakePlayer.transform);

            spawnedNameplates.Add(nameplate);
        }
    }
}