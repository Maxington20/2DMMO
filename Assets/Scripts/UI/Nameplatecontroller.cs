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

        CacheTargetComponents();
    }

    private void LateUpdate()
    {
        if (target == null || nameText == null)
        {
            Hide();
            return;
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera == null)
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

        nameText.text = ResolveDisplayName();

        Vector3 screenPosition = mainCamera.WorldToScreenPoint(target.position + worldOffset);
        transform.position = screenPosition;
    }

    public void Initialize(Transform newTarget)
    {
        SetTarget(newTarget);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        CacheTargetComponents();
    }

    public void SetName(string displayName)
    {
        if (nameText != null)
        {
            nameText.text = displayName;
        }
    }

    private void CacheTargetComponents()
    {
        targetHealth = target != null ? target.GetComponent<Health>() : null;
    }

    private string ResolveDisplayName()
    {
        if (target == null)
        {
            return string.Empty;
        }

        FakePlayerIdentity fakePlayerIdentity = target.GetComponent<FakePlayerIdentity>();

        if (fakePlayerIdentity != null)
        {
            return fakePlayerIdentity.DisplayName;
        }

        CombatEntity combatEntity = target.GetComponent<CombatEntity>();

        if (combatEntity != null)
        {
            return combatEntity.DisplayName;
        }

        QuestGiver questGiver = target.GetComponent<QuestGiver>();

        if (questGiver != null)
        {
            return questGiver.DisplayName;
        }

        Enemy enemy = target.GetComponent<Enemy>();

        if (enemy != null)
        {
            return enemy.EnemyName;
        }

        return target.name;
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