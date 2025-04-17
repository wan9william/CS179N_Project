using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 2f;
    public float damage = 10f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Bullet>() != null)
        {
            Debug.Log("[Bullet] Ignored collision with another bullet");
            return;
        }

        Debug.Log($"[Collision] Hit {collision.collider.name}");
        EnemyHealth enemy = collision.collider.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            Debug.Log($"[Bullet] Hit enemy with health: {enemy.name}");
            enemy.TakeDamage(damage);
        }

        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[Bullet] Triggered with {other.name}");

        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            Debug.Log($"[Bullet] Hit enemy with health: {enemy.name}");
            enemy.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}