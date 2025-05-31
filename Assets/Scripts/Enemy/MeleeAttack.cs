using UnityEngine;
using System.Collections;

public class MeleeAttack : MonoBehaviour, EnemyAttack
{
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
        Debug.Log("Attack!");
        Debug.Log(stats);
        if (target == null || stats == null || !canAttack || isAttacking) return;

        float distance = Vector2.Distance(transform.position, target.position);
        if (distance <= stats.attackRange)
        {
            Debug.Log("Try attack");
            StartCoroutine(PerformAttack(target, stats));
        }
    }

    IEnumerator PerformAttack(Transform target, EnemyStats stats)
    {
        isAttacking = true;
        yield return new WaitForSeconds(stats.attackDelay); // wind-up

        if (target.TryGetComponent<Player>(out var playerHealth))
        {
            playerHealth.TakeDamage(stats.attackDamage);
            Debug.Log("[MeleeAttack] Player hit!");
        }

        yield return new WaitForSeconds(stats.attackCooldown); // cooldown
        isAttacking = false;
    }
}
