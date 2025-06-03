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

        //PlayerHealth player = Object.FindFirstObjectByType<PlayerHealth>();

        //The above commented out line was what it was before. However, we want the player and the healthbar to be coupled.
        //Therefore, we have the player update its own healthbar, so any damage should be taken through the player.
        Player player = Object.FindFirstObjectByType<Player>();
        if (player != null)
        {
            Debug.Log($"[EnemyBullet] Hit player: {player.name}");
            player.TakeDamage((int)damage);
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("[EnemyBullet] No PlayerHealth found in target.");
        }
    }


}
