using TMPro;
using UnityEngine;

public class NameplateController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;
    [SerializeField] private TextMeshProUGUI nameText;

    [Header("Settings")]
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 1.1f, 0f);

    private Camera mainCamera;
    private Health targetHealth;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        mainCamera = Camera.main;
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        if (target != null)
        {
            targetHealth = target.GetComponent<Health>();
        }
    }

    private void LateUpdate()
    {
        if (target == null || nameText == null || mainCamera == null)
        {
            Hide();
            return;
        }

        if (targetHealth == null)
        {
            targetHealth = target.GetComponent<Health>();
        }

        if (targetHealth != null && targetHealth.IsDead)
        {
            Hide();
            return;
        }

        Show();

        Vector3 screenPosition = mainCamera.WorldToScreenPoint(target.position + worldOffset);
        transform.position = screenPosition;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        targetHealth = target != null ? target.GetComponent<Health>() : null;
    }

    public void SetName(string displayName)
    {
        if (nameText != null)
        {
            nameText.text = displayName;
        }
    }

    private void Show()
    {
        canvasGroup.alpha = 1f;
    }

    private void Hide()
    {
        canvasGroup.alpha = 0f;
    }
}