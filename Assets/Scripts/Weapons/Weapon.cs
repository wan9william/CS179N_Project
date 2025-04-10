using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Weapon : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireForce = 10f;
    [SerializeField] private float fireCooldown = 0.3f;


    private float lastFireTime = 0f;


    public void Shoot()
    {
        if (Time.time < lastFireTime + fireCooldown)
            return;


        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = firePoint.up * fireForce;


        lastFireTime = Time.time;
    }
}
