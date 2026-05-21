using UnityEngine;

public class FloatingCombatTextSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Canvas worldCanvas;
    [SerializeField] private FloatingCombatText floatingCombatTextPrefab;

    [Header("Settings")]
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 0.8f, 0f);

    public static FloatingCombatTextSpawner Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    public void SpawnDamageText(Vector3 worldPosition, int damageAmount)
    {
        if (mainCamera == null || worldCanvas == null || floatingCombatTextPrefab == null)
        {
            return;
        }

        Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition + worldOffset);

        FloatingCombatText textInstance = Instantiate(
            floatingCombatTextPrefab,
            worldCanvas.transform
        );

        textInstance.transform.position = screenPosition;
        textInstance.SetText(damageAmount.ToString());
    }
}