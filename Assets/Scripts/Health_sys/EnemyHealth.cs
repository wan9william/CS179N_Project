using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("UI References")]
    // The slider that shows the enemy's health
    [SerializeField] private Slider slider;
    // The camera for the canvas to face
    [SerializeField] private Camera mainCamera;
    // The enemy (or any target) that the bar follows
    [SerializeField] private Transform target;

    private Canvas canvas;

    void Awake()
    {
        // Get the Canvas component on this GameObject
        canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            // Set up the canvas for a world space UI
            canvas.renderMode = RenderMode.WorldSpace;
            // Auto-assign the main camera to the canvas if not set manually
            canvas.worldCamera = Camera.main;

            // Try to set the canvas sorting layer and order based on the enemy's SpriteRenderer (if available)
            SpriteRenderer enemySprite = GetComponentInParent<SpriteRenderer>();
            if (enemySprite != null)
            {
                canvas.sortingLayerID = enemySprite.sortingLayerID;
                // Make sure the health bar appears just above the enemy sprite
                canvas.sortingOrder = enemySprite.sortingOrder + 1;
            }
        }

        // Fallback assignments
        if (mainCamera == null)
            mainCamera = Camera.main;
        if (target == null)
            target = transform;

        // Auto-find the slider if it hasn't been set
        if (slider == null)
        {
            slider = GetComponentInChildren<Slider>();
        }
    }

    void Start()
    {
        // Initialize current health to max
        currentHealth = maxHealth;

        if (slider != null)
        {
            // Set up the slider values so the bar is full at start
            slider.maxValue = maxHealth;
            slider.value = currentHealth;
        }
    }

    void Update()
    {
        // TESTING CODE:
        // Press Z to simulate taking 10 damage
        if (Input.GetKeyDown(KeyCode.Z))
        {
            TakeDamage(10f);
            Debug.Log("Z pressed: 10 damage applied");
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"[Enemy] {gameObject.name} took {amount} damage — remaining HP: {currentHealth}");

        if (slider != null)
        {
            slider.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy died!");
        Destroy(gameObject);
    }

    void LateUpdate()
    {
        // Make the health bar follow the target (e.g., above the enemy's head)
        if (target != null)
        {
            transform.position = target.position + Vector3.up * 0.5f;
        }

        // Ensure the health bar always faces the main camera
        if (mainCamera != null)
        {
            transform.rotation = mainCamera.transform.rotation;
        }
    }

    // Optional setter methods for dynamic assignment
    public void SetTarget(Transform followTarget) => target = followTarget;
    public void SetCamera(Camera cam) => mainCamera = cam;
}
