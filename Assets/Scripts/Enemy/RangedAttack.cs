using UnityEngine;
using System.Collections;

public class RangedAttack : MonoBehaviour, EnemyAttack
{
    public Transform shootPoint;
    private bool isAttacking = false;
    private bool canAttack = false;

    void Start()
    {
        StartCoroutine(EnableAfterInitialDelay());
    }

    IEnumerator EnableAfterInitialDelay()
    {
        yield return new WaitForSeconds(GetComponent<EnemyAI>().stats.initialAttackDelay);
        canAttack = true;
    }

    public void TryAttack(Transform target, EnemyStats stats)
    {
        if (target == null || stats == null || !canAttack || isAttacking) return;

        float distance = Vector2.Distance(transform.position, target.position);
        if (distance <= stats.attackRange)
        {
            StartCoroutine(PerformAttack(target, stats));
        }
    }

    IEnumerator PerformAttack(Transform target, EnemyStats stats)
    {
        isAttacking = true;
        yield return new WaitForSeconds(stats.attackDelay); // wind-up

        Vector2 spawnPos = shootPoint ? shootPoint.position : transform.position;
        Vector2 direction = (target.position - (Vector3)spawnPos).normalized;

        GameObject proj = Instantiate(stats.projectilePrefab, spawnPos, Quaternion.identity);
        if (proj.TryGetComponent<Rigidbody2D>(out var rb))
            rb.linearVelocity = direction * stats.projectileSpeed;

        if (proj.TryGetComponent<Bullet>(out var bullet))
        {
            bullet.damage = stats.attackDamage;
        }

        Debug.Log("[RangedAttack] Fired projectile at player.");

        yield return new WaitForSeconds(stats.attackCooldown);
        isAttacking = false;
    }
}
