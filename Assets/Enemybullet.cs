using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float lifetime = 2f;
    public float damage = 10f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[EnemyBullet] Triggered with {other.name}");

        // Only affect player
        if (other.TryGetComponent<PlayerHealth>(out var player))
        {
            Debug.Log($"[EnemyBullet] Hit player: {player.name}");
            player.TakeDamage((int)damage);
            Destroy(gameObject);
        }
    }
}
