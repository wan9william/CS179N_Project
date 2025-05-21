using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public enum FireMode
{
    SemiAuto,
    FullAuto
}

public class Weapon : Equippable
{
    [SerializeField] private Transform weaponOwner; // Reference to player or owner

    [Header("General Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireForce = 10f;
    [SerializeField] private float fireCooldown = 0.3f;
    [SerializeField] private FireMode fireMode = FireMode.SemiAuto;
    [SerializeField] private bool chamberReset = false;
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
    [SerializeField] private Item_ScriptableObj ammoType;
    [SerializeField] private int magazineSize = 10;
    [SerializeField] private float reloadTime = 1.5f;

    [Header("Animation Settings")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    [Header("Object Manager")]
    [SerializeField] private ObjectManager objectManager;


    private float currentSpread = 0f;
    private float lastFireTime = 0f;

    private int currentAmmo = -1;
    private bool isReloading = false;

    private IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("[Weapon] Reloading...");

        yield return new WaitForSeconds(reloadTime);

        Inventory inv = Player.Singleton.getInventory();
        int ammoNeeded = magazineSize - currentAmmo;
        int availableAmmo = inv.GetTotalAmmo(ammoType);

        int ammoToLoad = Mathf.Min(ammoNeeded, availableAmmo);

        if (ammoToLoad > 0)
        {
            inv.ConsumeAmmo(ammoType, ammoToLoad);
            currentAmmo += ammoToLoad;
            Debug.Log($"[Weapon] Reloaded {ammoToLoad} bullet(s). Now: {currentAmmo}/{magazineSize}");
        }
        else
        {
            Debug.Log("[Weapon] No ammo available to reload.");
        }

        InventorySlot[] slots = inv.getInventorySlots();
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].getFab()?.name == gameObject.name.Replace("(Clone)", "").Trim())
            {
                slots[i].storedAmmo = currentAmmo;
                slots[i].UpdateQuantityDisplay();
                break;
            }
        }

        isReloading = false;
    }

    void Start()
    {
        if(objectManager == null)
            objectManager = GameObject.FindWithTag("Object_Manager").GetComponent<ObjectManager>();

        // ✅ Only assign full ammo if not restored yet
        if (currentAmmo < 0)
            currentAmmo = magazineSize;

        if (weaponOwner == null)
            weaponOwner = transform.root;
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

    public override void Use(ref Player player) {
        Shoot();
    }

    public bool Shoot()
    {
        // Check if within firerate
        if (Time.time < lastFireTime + fireCooldown)
        {
            return false;
        }
        if (currentAmmo <= 0)
        {
            Debug.Log("[Weapon] Out of ammo! Reload required.");
            // StartCoroutine(Reload());
            return false;
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
            GameObject bullet = objectManager.RequestBulletObj();//Instantiate(projectilePrefab, firePoint.position, bulletRotation);
            bullet.transform.rotation = bulletRotation;
            bullet.transform.position = firePoint.position;

            // Assign velocity
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.linearVelocity = bullet.transform.right * fireForce;

            // ✅ Assign damage
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.damage = (int)damage;
            }
        
            animator.SetTrigger("Shoot");
            }

        // Bloom increases with each shot (not each pellet)
        currentAmmo--;
        InventorySlot[] slots = Player.Singleton.getInventory().getInventorySlots();
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].getFab().name == gameObject.name.Replace("(Clone)", "").Trim())
            {
                slots[i].UpdateAmmoDisplay(currentAmmo, magazineSize);
                break;
            }
        }

        currentSpread = Mathf.Min(currentSpread + bloomIncreasePerShot, maxSpreadAngle);
        lastFireTime = Time.time;
        
        return true;
    }

    public void StartReload()
    {
        if (isReloading || currentAmmo >= magazineSize)
                return;

            Inventory inv = Player.Singleton.getInventory();
            if (inv.GetTotalAmmo(ammoType) > 0)
            {
                Player.Singleton.StartCoroutine(Reload());
            }
            else
            {
                Debug.Log("[Weapon] No ammo to reload.");
            }
    }

    public bool IsReloading()
    {
        return isReloading;
    }

    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }

    public void SetCurrentAmmo(int value)
    {
        currentAmmo = Mathf.Clamp(value, 0, magazineSize);
    }

    public int GetMagazineSize()
    {
        return magazineSize;
    }
}