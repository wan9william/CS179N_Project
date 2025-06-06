using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] GameObject _explosion;
    public float lifetime = 2f;
    public float damage = 10f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[EnemyBullet] Triggered with {other.name}");
        bool success = true;

        if (other.gameObject.GetComponent<EnemyBullet>()) success = false;
        if (other.gameObject.GetComponent<EnemyHealth>()) success = false;
        //PlayerHealth player = Object.FindFirstObjectByType<PlayerHealth>();

        //The above commented out line was what it was before. However, we want the player and the healthbar to be coupled.
        //Therefore, we have the player update its own healthbar, so any damage should be taken through the player.
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            success = true;
            Debug.Log($"[EnemyBullet] Hit player: {player.name}");
            player.TakeDamage((int)damage);
        }
        else
        {
            Debug.Log("[EnemyBullet] No PlayerHealth found in target.");
        }

        Interactable _itemInteractable = other.GetComponent<Interactable>();
        if (_itemInteractable) { success |= _itemInteractable.Hit(damage); }

        if (success)
        {
            gameObject.SetActive(false);
            Instantiate(_explosion, gameObject.transform);
            Destroy(gameObject);
        }
    }


}
