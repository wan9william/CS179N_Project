using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Seeker), typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    [Header("Enemy Stats")]
    public EnemyStats stats;                // Holds all movement and attack values
    public Transform target;                // Assign the player here
    public SpriteRenderer spriteRenderer;   // Auto-assigned if not set

    private Seeker seeker;
    private Rigidbody2D rb;
    private Path path;
    private Vector2 currentDirection;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath = false;

    private EnemyAttack attackBehavior;     // Reference to Melee or Ranged attack script
    private EnemyState currentState = EnemyState.Chase;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        attackBehavior = GetComponent<EnemyAttack>(); // Tries to find Melee or RangedAttack
        currentDirection = Vector2.zero;

        // Recalculate path to target every 0.2 seconds
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
    }

    void HandleChase()
    {
        if (path == null || currentWaypoint >= path.vectorPath.Count)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Try to attack the player
        if (attackBehavior != null)
        {
            attackBehavior.TryAttack(target, stats); 
        }

        // Stop movement if close enough to player
        if (Vector2.Distance(rb.position, target.position) <= stats.stopDistance)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Move toward next path point
        Vector2 targetDirection = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        currentDirection = Vector2.Lerp(currentDirection, targetDirection, 0.2f);
        Vector2 nextPosition = rb.position + currentDirection * stats.moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(nextPosition);

        // Flip sprite left/right based on movement direction
        if (stats.flipSprite && spriteRenderer != null)
        {
            spriteRenderer.flipX = currentDirection.x < -0.05f;
        }

        // Check if we're close enough to next waypoint
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < stats.nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }
}
