using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyStats", menuName = "Enemies/Enemy Stats")]
public class EnemyStats : ScriptableObject
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float stopDistance = 1.5f;
    public float nextWaypointDistance = 0.3f;
    public bool flipSprite = true;

    [Header("Attack Settings")]
    public float attackRange = 1f;
    public float attackCooldown = 1f;
    public int attackDamage = 10;

    [Header("Ranged Only")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
}
