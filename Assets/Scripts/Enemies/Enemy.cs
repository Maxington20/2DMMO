using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Info")]
    [SerializeField] private string enemyName = "Wolf";

    public string EnemyName => enemyName;
}