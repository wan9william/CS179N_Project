using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Canvas))]
public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("UI References")]
    [SerializeField] private Slider slider;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform target;

    private EnemyAI aiscript;

    [Header("Animation Settings")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    private Canvas canvas;

    void Awake()
    {
        //Prevent running this script on the Player
        if (CompareTag("Player"))
        {
            Debug.LogWarning("[EnemyHealth] Script is attached to Player. Removing to avoid conflict.");
            Destroy(this);
            return;
        }

        // Get canvas component and configure
        canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;

            // Match enemy sprite sorting order
            SpriteRenderer sprite = GetComponentInParent<SpriteRenderer>();
            if (sprite != null)
            {
                canvas.sortingLayerID = sprite.sortingLayerID;
                canvas.sortingOrder = sprite.sortingOrder + 1;
            }
        }

        // Safe fallbacks
        if (mainCamera == null) mainCamera = Camera.main;
        if (target == null) target = transform;
        if (slider == null) slider = GetComponentInChildren<Slider>();

        if(!animator) animator = GetComponent<Animator>();
        if (!aiscript) aiscript= GetComponent<EnemyAI>();
    }

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

        animator.SetTrigger("Damaged");
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (slider != null)
            slider.value = currentHealth;

        Debug.Log($"[EnemyHealth] {gameObject.name} took {amount} damage. Remaining HP: {currentHealth}");

        if (currentHealth <= 0f)
        {
            animator.SetTrigger("Death");
            aiscript.SetDeadState();
        }
    }

    void Die()
    {
        Debug.Log($"[EnemyHealth] {gameObject.name} has died.");
        Destroy(gameObject);
    }

    void LateUpdate()
    {
        if (mainCamera != null)
        {
            transform.rotation = mainCamera.transform.rotation;
        }

        // Optional: follow above enemy head
        // transform.position = target.position + Vector3.up * 0.5f;
    }

    // Optional dynamic setters
    public void SetTarget(Transform followTarget) => target = followTarget;
    public void SetCamera(Camera cam) => mainCamera = cam;
}
