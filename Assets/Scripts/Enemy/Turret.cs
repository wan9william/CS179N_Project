using UnityEngine;

public class TurretBehavior : MonoBehaviour
{
    [Header("Detection Settings")]
    public Transform player;
    public float detectionRange = 10f;
    public float fieldOfViewAngle = 90f;
    public LayerMask obstructionMask;

    [Header("Turret Rotation")]
    public float rotationSpeed = 180f; // Degrees per second

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1f;
    public float bulletSpeed = 10f;

    private float nextFireTime = 0f;
    private bool playerVisible = false;

    void Update()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        playerVisible = IsPlayerVisible();

        if (playerVisible)
        {
            RotateTowardPlayer();

            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }
        else
        {
            // Optional: scanning idle rotation
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }
    }

    void RotateTowardPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Use Quaternion to rotate smoothly
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle - 90f); // Adjust -90 if your turret points up
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        if (bullet.TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.linearVelocity = firePoint.up * bulletSpeed;
        }
    }

    bool IsPlayerVisible()
    {
        Vector2 toPlayer = player.position - transform.position;

        if (toPlayer.magnitude > detectionRange)
            return false;

        float angleToPlayer = Vector2.Angle(transform.up, toPlayer.normalized); // Assumes turret faces up
        if (angleToPlayer > fieldOfViewAngle / 2f)
            return false;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, toPlayer.normalized, detectionRange, obstructionMask);
        if (hit.collider != null && hit.collider.transform != player)
            return false;

        return true;
    }
}
