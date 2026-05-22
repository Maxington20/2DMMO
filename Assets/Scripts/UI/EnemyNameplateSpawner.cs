using UnityEngine;

public class EnemyNameplateSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyNameplateUI nameplatePrefab;
    [SerializeField] private Canvas worldCanvas;

    private void Start()
    {
        SpawnNameplates();
    }

    private void SpawnNameplates()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();

        foreach (Enemy enemy in enemies)
        {
            EnemyNameplateUI nameplate = Instantiate(
                nameplatePrefab,
                worldCanvas.transform
            );

            nameplate.Initialize(enemy.transform);
        }
    }
}