using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyNameplateUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;
    [SerializeField] private Enemy enemy;
    [SerializeField] private Health health;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image healthFill;

    [Header("Position")]
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 1.2f, 0f);

    private Camera mainCamera;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        mainCamera = Camera.main;
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void Update()
    {
        if (target == null || enemy == null || health == null)
        {
            Hide();
            return;
        }

        if (health.IsDead)
        {
            Hide();
            return;
        }

        Show();

        Vector3 screenPosition = mainCamera.WorldToScreenPoint(target.position + worldOffset);
        transform.position = screenPosition;

        if (nameText != null)
        {
            nameText.text = $"Level {enemy.Level} {enemy.EnemyName}";
        }

        if (healthFill != null)
        {
            healthFill.fillAmount = (float)health.CurrentHealth / health.MaxHealth;
        }
    }

    public void Initialize(Transform targetTransform)
    {
        target = targetTransform;

        if (target != null)
        {
            enemy = target.GetComponent<Enemy>();
            health = target.GetComponent<Health>();
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