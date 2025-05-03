using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Seeker), typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    [Header("Settings")]
    public EnemySettings settings;
    public Transform target;
    public SpriteRenderer spriteRenderer;

    /*
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float waitTimeAtPoint = 1f;

    private int patrolIndex = 0;
    private float waitTimer = 0f;
    */

    private Seeker seeker;
    private Rigidbody2D rb;
    private Path path;
    private Vector2 currentDirection;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath = false;

    private EnemyState currentState = EnemyState.Chase; // Start directly chasing

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

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

            /*
            case EnemyState.Patrol:
                HandlePatrol();
                break;
            */

            case EnemyState.Chase:
                HandleChase();
                break;
        }
    }

    /*
    void HandlePatrol()
    {
        if (patrolPoints.Length == 0)
            return;

        Transform point = patrolPoints[patrolIndex];
        float distance = Vector2.Distance(rb.position, point.position);

        if (distance <= 0.2f)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtPoint)
            {
                patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
                waitTimer = 0f;
            }

            rb.velocity = Vector2.zero;
            return;
        }

        Vector2 direction = ((Vector2)point.position - rb.position).normalized;
        currentDirection = Vector2.Lerp(currentDirection, direction, 0.2f);
        Vector2 nextPosition = rb.position + currentDirection * settings.speed * Time.fixedDeltaTime;
        rb.MovePosition(nextPosition);

        if (settings.flipSprite && spriteRenderer != null)
        {
            spriteRenderer.flipX = currentDirection.x < -0.05f;
        }
    }
    */

    void HandleChase()
    {
        if (path == null || currentWaypoint >= path.vectorPath.Count)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (Vector2.Distance(rb.position, target.position) <= settings.stopDistance)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 targetDirection = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        currentDirection = Vector2.Lerp(currentDirection, targetDirection, 0.2f);
        Vector2 nextPosition = rb.position + currentDirection * settings.speed * Time.fixedDeltaTime;
        rb.MovePosition(nextPosition);

        if (settings.flipSprite && spriteRenderer != null)
        {
            spriteRenderer.flipX = currentDirection.x < -0.05f;
        }

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < settings.nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }
}
