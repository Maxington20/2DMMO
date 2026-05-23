using UnityEngine;

[RequireComponent(typeof(Health))]
public class CombatEntity : MonoBehaviour
{
    [Header("Entity Info")]
    [SerializeField] private string displayName = "Combat Entity";
    [SerializeField] private bool canBeTargetedByEnemies = true;

    private Health health;

    public string DisplayName => displayName;
    public bool CanBeTargetedByEnemies => canBeTargetedByEnemies;
    public Health Health => health;
    public bool IsAlive => health != null && !health.IsDead;

    private void Awake()
    {
        health = GetComponent<Health>();
    }
}