using UnityEngine;

public class RangedAttack : MonoBehaviour, EnemyAttack
{
    private float lastAttackTime = -Mathf.Infinity;

    public void TryAttack(Transform target, EnemyStats stats)
    {
        if (target == null || stats == null || stats.projectilePrefab == null)
            return;

        // Check attack cooldown
        if (Time.time - lastAttackTime < stats.attackCooldown)
            return;

        // Only attack if within range
        float distance = Vector2.Distance(transform.position, target.position);
        if (distance > stats.attackRange)
            return;

        // Spawn projectile from enemy's position
        Vector2 spawnPos = transform.position;
        Vector2 direction = (target.position - transform.position).normalized;

        GameObject proj = Instantiate(stats.projectilePrefab, spawnPos, Quaternion.identity);

        if (proj.TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.linearVelocity = direction * stats.projectileSpeed;
        }

        lastAttackTime = Time.time;
    }
}
