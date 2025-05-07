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
    [SerializeField] private Transform weaponOwner; // Reference to player or owner

    [Header("General Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireForce = 10f;
    [SerializeField] private float fireCooldown = 0.3f;
    [SerializeField] private FireMode fireMode = FireMode.SemiAuto;
    [SerializeField] private float damage = 10f;

    [Header("Bloom Settings")]
    [SerializeField] private float baseSpreadAngle = 0f;
    [SerializeField] private float maxSpreadAngle = 10f;
    [SerializeField] private float bloomIncreasePerShot = 1f;
    [SerializeField] private float bloomDecaySpeed = 3f;

    [Header("Multi-Bullet Settings")]
    [SerializeField] private int bulletsPerShot = 1; // 1 = normal gun, 6 = shotgun
    [SerializeField] private float perBulletSpread = 5f; // angle spread between bullets

    [Header("Ammo Settings")]
    [SerializeField] private int magazineSize = 10;
    [SerializeField] private float reloadTime = 1.5f;

    [Header("Animation Settings")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;


    private float currentSpread = 0f;
    private float lastFireTime = 0f;

    private int currentAmmo;
    private bool isReloading = false;

    void Start()
    {
        currentAmmo = magazineSize;
        if (weaponOwner == null)
            weaponOwner = transform.root; // auto-assign the top-level parent if needed
    }   

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
    // Check if within firerate
    if (Time.time < lastFireTime + fireCooldown)
    {
        return;
    }
    if (currentAmmo <= 0)
    {
        Debug.Log("[Weapon] Out of ammo! Reload required.");
        StartCoroutine(Reload());
        return;
    }

    Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    Vector2 shootDirection = (mouseWorldPos - weaponOwner.position).normalized;
    float baseAngle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;

    for (int i = 0; i < bulletsPerShot; i++)
    {
        // Spread = both random bloom and fixed pellet spread
        float bloom = Random.Range(-currentSpread, currentSpread);
        float spread = Random.Range(-perBulletSpread, perBulletSpread);
        float finalAngle = baseAngle + bloom + spread;

        Quaternion bulletRotation = Quaternion.Euler(0f, 0f, finalAngle);
        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, bulletRotation);

        // Assign velocity
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = bullet.transform.right * fireForce;

        // ✅ Assign damage
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.damage = damage;
        }
        
        animator.SetTrigger("Shoot");
        }

    // Bloom increases with each shot (not each pellet)
    currentAmmo--;
    currentSpread = Mathf.Min(currentSpread + bloomIncreasePerShot, maxSpreadAngle);
    lastFireTime = Time.time;
}

private IEnumerator Reload()
{
    isReloading = true;
    Debug.Log("[Weapon] Reloading...");

    yield return new WaitForSeconds(reloadTime);

    currentAmmo = magazineSize;
    isReloading = false;
    Debug.Log("[Weapon] Reloaded.");
}
}
