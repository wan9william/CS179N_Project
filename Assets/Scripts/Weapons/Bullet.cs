using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bullet : MonoBehaviour
{
    public float lifetime = 2f;


    void Start()
    {
        Destroy(gameObject, lifetime);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(gameObject);
    }
}
