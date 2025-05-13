using UnityEngine;

public interface EnemyAttack
{
    void TryAttack(Transform target, EnemyStats stats);
}
