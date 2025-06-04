using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Seeker), typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource movementAudioSource;
    [SerializeField] private AudioClip movementClip;

    [Header("Enemy Stats")]
    public EnemyStats stats;
    public Transform target;
    public SpriteRenderer spriteRenderer;

    private Seeker seeker;
    private Rigidbody2D rb;
    private Path path;
    private Vector2 currentDirection;
    private int currentWaypoint = 0;

    private EnemyAttack attackBehavior;

    public EnemyState currentState = EnemyState.Idle;

    // You can adjust this threshold to fit how close the player must be
    public float activationDistance = 5f;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) target = playerObj.transform;
        }

        attackBehavior = GetComponent<EnemyAttack>();
        currentDirection = Vector2.zero;

        InvokeRepeating(nameof(UpdatePath), 0f, 0.2f);
    }

    void UpdatePath()
    {
        if (seeker.IsDone() && target != null && currentState == EnemyState.Chase)
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    void FixedUpdate()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                rb.linearVelocity = Vector2.zero;
                break;

            case EnemyState.Chase:
                HandleChase();
                break;
        }

        CheckPlayerProximity();
    }

    void HandleChase()
    {
        if (path == null || currentWaypoint >= path.vectorPath.Count)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (Vector2.Distance(rb.position, target.position) <= stats.stopDistance)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (attackBehavior != null)
        {
            attackBehavior.TryAttack(target, stats);
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        currentDirection = Vector2.Lerp(currentDirection, direction, 0.2f);

        Vector2 movement = currentDirection * stats.moveSpeed;
        rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);

        if (movementAudioSource && movementClip && !movementAudioSource.isPlaying)
        {
            movementAudioSource.clip = movementClip;
            movementAudioSource.loop = true;
            movementAudioSource.Play();
        }

        if (stats.flipSprite && spriteRenderer != null)
        {
            spriteRenderer.flipX = currentDirection.x < -0.05f;
        }

        float distanceToWaypoint = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distanceToWaypoint < stats.nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    void CheckPlayerProximity()
    {
        if (target == null) return;

        float distance = Vector2.Distance(rb.position, target.position);

        if (distance <= activationDistance && currentState == EnemyState.Idle)
        {
            currentState = EnemyState.Chase;
        }
        else if (currentState == EnemyState.Chase && distance > activationDistance + 2f)
        {
            currentState = EnemyState.Idle;
        }
    }
}
