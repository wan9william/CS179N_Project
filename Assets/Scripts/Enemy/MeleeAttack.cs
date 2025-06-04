using UnityEngine;
using System.Collections;

public class MeleeAttack : MonoBehaviour, EnemyAttack
{
    private bool isAttacking = false;
    private bool canAttack = false;
    [SerializeField] float dist;

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
        Debug.Log("Attack!");
        Debug.Log(stats);
        if (target == null || stats == null || !canAttack || isAttacking) return;

        float distance = Vector2.Distance(transform.position, target.position);
        dist = distance;
        if (distance <= stats.attackRange)
        {
            Debug.Log("[MeleeAttack] Within range, starting attack...");
            StartCoroutine(PerformAttack(target, stats));
        }
    }

    IEnumerator PerformAttack(Transform target, EnemyStats stats)
    {
        isAttacking = true;

        // Optional wind-up delay
        yield return new WaitForSeconds(stats.attackDelay);

        if (target.TryGetComponent<Player>(out var playerHealth))
        {
            playerHealth.TakeDamage(stats.attackDamage);
            Debug.Log("[MeleeAttack] Player hit!");
        }
        else
        {
            Debug.LogWarning("[MeleeAttack] Could not find PlayerHealth on target.");
        }

        // Cooldown before next possible attack
        yield return new WaitForSeconds(stats.attackCooldown);
        isAttacking = false;
    }
}
