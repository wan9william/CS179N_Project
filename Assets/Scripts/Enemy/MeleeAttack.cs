using UnityEngine;

public class MeleeAttack : MonoBehaviour, EnemyAttack
{
    private float lastAttackTime = -Mathf.Infinity;

    public void TryAttack(Transform target, EnemyStats stats)
    {
        if (target == null || stats == null) return;

        if (Time.time - lastAttackTime < stats.attackCooldown)
            return;

        float distance = Vector2.Distance(transform.position, target.position);
        if (distance <= stats.attackRange)
        {
            if (target.TryGetComponent<PlayerHealth>(out var playerHealth))
            {
                playerHealth.TakeDamage(stats.attackDamage);
                Debug.Log("Melee attack hit!");
            }

            lastAttackTime = Time.time;
        }
    }
}
