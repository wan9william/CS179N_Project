using UnityEngine;

public class RangedAttack : MonoBehaviour, EnemyAttack
{
    public EnemyStats stats;
    public Transform shootPoint;

    private float lastAttackTime = -Mathf.Infinity;

    public void TryAttack(Transform target, EnemyStats _stats) // ✅ Match interface
    {
        if (target == null || stats.projectilePrefab == null || shootPoint == null)
            return;

        if (Time.time - lastAttackTime < stats.attackCooldown)
            return;

        float distance = Vector2.Distance(transform.position, target.position);
        if (distance <= stats.attackRange)
        {
            Vector2 direction = (target.position - shootPoint.position).normalized;
            GameObject proj = Instantiate(stats.projectilePrefab, shootPoint.position, Quaternion.identity);

            Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = direction * stats.projectileSpeed;

            Debug.Log("Ranged attack fired!");
            lastAttackTime = Time.time;
        }
    }
}

