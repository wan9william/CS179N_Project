using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 2f;
    public float damage = 10f;
    private float elapsedTime = 0f;

    void Start()
    {
    }

    void Update() {
        elapsedTime += Time.deltaTime;
        if (elapsedTime > lifetime) gameObject.SetActive(false);
    }

    public void ResetTimer() {
        elapsedTime = 0f;
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

        
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[Bullet] Triggered with {other.name}");

        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        bool success = false;

        //This checks if the hit item is of the interactable class. If it is, it means it has health and can be destroyed. Hit appropriately
        Interactable _itemInteractable = other.GetComponent<Interactable>();
        if (_itemInteractable) { success = _itemInteractable.Hit(damage); }
        //////////////


        if (enemy != null)
        {
            Debug.Log($"[Bullet] Hit enemy with health: {enemy.name}");
            enemy.TakeDamage(damage);
            success = true;
        }

        if(success) gameObject.SetActive(false);
    }
}