using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTargetingController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject targetIndicator;

    private Enemy currentTarget;

    public Enemy CurrentTarget => currentTarget;

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        HandleMouseTargeting();
    }

    private void HandleMouseTargeting()
    {
        Mouse mouse = Mouse.current;

        if (mouse == null || !mouse.leftButton.wasPressedThisFrame)
        {
            return;
        }

        Vector2 mouseScreenPosition = mouse.position.ReadValue();
        Vector2 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);

        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero);

        if (hit.collider == null)
        {
            ClearTarget();
            return;
        }

        Enemy enemy = hit.collider.GetComponent<Enemy>();

        if (enemy == null)
        {
            ClearTarget();
            return;
        }

        SetTarget(enemy);
    }

    private void SetTarget(Enemy enemy)
    {
        currentTarget = enemy;

        targetIndicator.SetActive(true);

        Vector3 indicatorPosition = enemy.transform.position;
        indicatorPosition.y -= 0.45f;

        targetIndicator.transform.position = indicatorPosition;
    }

    private void ClearTarget()
    {
        currentTarget = null;

        if (targetIndicator != null)
        {
            targetIndicator.SetActive(false);
        }
    }
}