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

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (target == null || nameText == null || mainCamera == null)
        {
            return;
        }

        Vector3 screenPosition = mainCamera.WorldToScreenPoint(target.position + worldOffset);
        transform.position = screenPosition;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetName(string displayName)
    {
        if (nameText != null)
        {
            nameText.text = displayName;
        }
    }
}