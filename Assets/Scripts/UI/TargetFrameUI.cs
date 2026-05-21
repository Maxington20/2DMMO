using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TargetFrameUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject frameRoot;
    [SerializeField] private TextMeshProUGUI enemyNameText;
    [SerializeField] private Image healthBarFill;

    private Enemy currentTarget;
    private Health currentHealth;

    private void Update()
    {
        if (currentTarget == null || currentHealth == null)
        {
            frameRoot.SetActive(false);
            return;
        }

        if (!currentTarget.gameObject.activeInHierarchy || currentHealth.IsDead)
        {
            ClearTarget();
            return;
        }

        frameRoot.SetActive(true);

        enemyNameText.text = currentTarget.EnemyName;

        healthBarFill.fillAmount =
            (float)currentHealth.CurrentHealth / currentHealth.MaxHealth;
    }

    public void SetTarget(Enemy enemy)
    {
        currentTarget = enemy;

        if (enemy == null)
        {
            currentHealth = null;
            return;
        }

        currentHealth = enemy.GetComponent<Health>();
    }

    public void ClearTarget()
    {
        currentTarget = null;
        currentHealth = null;
        frameRoot.SetActive(false);
    }
}