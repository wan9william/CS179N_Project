using UnityEngine;

public class TurretBehavior : MonoBehaviour
{
    [Header("Detection Settings")]
    public Transform player;
    public float detectionRange = 10f;
    public float fieldOfViewAngle = 90f;
    public LayerMask obstructionMask;

    [Header("Turret Rotation")]
    public float rotationSpeed = 30f; // degrees per second

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1f;
    public float bulletSpeed = 10f;

    private float nextFireTime = 0f;
    private bool playerVisible = false;

    void Update()
    {
        if (player == null) return;

        playerVisible = IsPlayerVisible();

        if (playerVisible)
        {
            // Rotate toward player
            Vector2 direction = (player.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, angle), rotationSpeed * Time.deltaTime);

            if (Time.time >= nextFireTime)
            {
                Shoot(direction);
                nextFireTime = Time.time + fireRate;
            }
        }
        else
        {
            // Idle scanning (rotate slowly when no player in sight)
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }
    }

    void Shoot(Vector2 direction)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        if (bullet.TryGetComponent<Rigidbody2D>(out var rb))
            rb.linearVelocity = direction * bulletSpeed;
    }

    bool IsPlayerVisible()
    {
        Vector2 toPlayer = player.position - transform.position;

        if (toPlayer.magnitude > detectionRange)
            return false;

        float angleToPlayer = Vector2.Angle(transform.right, toPlayer.normalized);
        if (angleToPlayer > fieldOfViewAngle / 2f)
            return false;

        // Check if player is behind wall or obstacle
        RaycastHit2D hit = Physics2D.Raycast(transform.position, toPlayer.normalized, detectionRange, obstructionMask);
        if (hit.collider != null && hit.collider.transform != player)
            return false;

        return true;
    }
}
