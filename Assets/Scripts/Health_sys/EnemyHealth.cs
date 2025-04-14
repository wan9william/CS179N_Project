using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    //Usage for damage
    //collision.GetComponentInChildren<EnemyHealth>()?.TakeDamage(10f);
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("UI")]
    [SerializeField] private Slider slider;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform target;

    void Start()
    {
        currentHealth = maxHealth;

        if (slider != null)
        {
            slider.maxValue = maxHealth;
            slider.value = currentHealth;
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (slider != null)
        {
            slider.value = currentHealth;
        }
    }

    void Die()
    {
        // Destroy the enemy or play animation
        Debug.Log("Enemy died!");
        Destroy(gameObject); // You can replace with death animation later
    }

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + Vector3.up * 1.5f;
        }

        if (mainCamera != null)
        {
            transform.rotation = mainCamera.transform.rotation;
        }
    }

    public void SetTarget(Transform followTarget)
    {
        target = followTarget;
    }

    public void SetCamera(Camera cam)
    {
        mainCamera = cam;
    }
}
