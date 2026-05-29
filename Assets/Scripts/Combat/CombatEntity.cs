using UnityEngine;

[RequireComponent(typeof(Health))]
public class CombatEntity : MonoBehaviour
{
    [Header("Entity Info")]
    [SerializeField] private string displayName = "Combat Entity";
    [SerializeField] private bool canBeTargetedByEnemies = true;

    private Health health;
    private FakePlayerIdentity fakePlayerIdentity;

    public string DisplayName
    {
        get
        {
            if (fakePlayerIdentity != null)
            {
                return fakePlayerIdentity.DisplayName;
            }

            return displayName;
        }
    }

    public bool CanBeTargetedByEnemies => canBeTargetedByEnemies;
    public Health Health => health;
    public bool IsAlive => health != null && !health.IsDead;

    private void Awake()
    {
        health = GetComponent<Health>();
        fakePlayerIdentity = GetComponent<FakePlayerIdentity>();
    }

    public void SetDisplayName(string newDisplayName)
    {
        if (!string.IsNullOrWhiteSpace(newDisplayName))
        {
            displayName = newDisplayName;
        }
    }
}