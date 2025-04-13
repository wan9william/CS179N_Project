using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FireMode
{
    SemiAuto,
    FullAuto
}

public class Weapon : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireForce = 10f;
    [SerializeField] private float fireCooldown = 0.3f;
    [SerializeField] private FireMode fireMode = FireMode.SemiAuto;

    [Header("Bloom Settings")]
    [SerializeField] private float baseSpreadAngle = 0f;
    [SerializeField] private float maxSpreadAngle = 10f;
    [SerializeField] private float bloomIncreasePerShot = 1f;
    [SerializeField] private float bloomDecaySpeed = 3f;

    private float currentSpread = 0f;
    private float lastFireTime = 0f;

    void Update()
    {
        // Bloom decay
        currentSpread = Mathf.MoveTowards(currentSpread, baseSpreadAngle, Time.deltaTime * bloomDecaySpeed);
    }

    public FireMode GetFireMode()
    {
        return fireMode;
    }

    public void Shoot()
    {
        if (Time.time < lastFireTime + fireCooldown)
            return;

        // Angle with bloom
        float spread = Random.Range(-currentSpread, currentSpread);
        Quaternion spreadRotation = Quaternion.Euler(0, 0, firePoint.eulerAngles.z + spread);

        // Spawn bullet
        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, spreadRotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = bullet.transform.up * fireForce;

        // Increase bloom
        currentSpread = Mathf.Min(currentSpread + bloomIncreasePerShot, maxSpreadAngle);

        // Update fire time
        lastFireTime = Time.time;
    }
}
