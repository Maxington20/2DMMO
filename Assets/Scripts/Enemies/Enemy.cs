using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Info")]
    [SerializeField] private string enemyName = "Wolf";
    [SerializeField] private int level = 1;

    public string EnemyName => enemyName;
    public int Level => level;
}