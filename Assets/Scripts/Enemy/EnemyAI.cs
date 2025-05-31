using UnityEngine;
using Pathfinding;
using System.Collections.Generic;

[RequireComponent(typeof(Seeker), typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    [Header("Enemy Stats")]
    public EnemyStats stats;
    public Transform target;
    public SpriteRenderer spriteRenderer;

    private Seeker seeker;
    private Rigidbody2D rb;
    private Path path;
    private int currentWaypoint = 0;
    private Vector2 currentDirection;

    private EnemyAttack attackBehavior;
    public EnemyState currentState = EnemyState.Idle;

    private List<Vector3> patrolPoints = new List<Vector3>();
    private int patrolIndex = 0;
    private float waitTimeAtPoint = 1f;
    private float waitTimer = 0f;
    private bool isWaiting = false;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer ??= GetComponent<SpriteRenderer>();

        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                target = playerObj.transform;
        }

        attackBehavior = GetComponent<EnemyAttack>();
        currentDirection = Vector2.zero;

        InvokeRepeating(nameof(UpdatePath), 0f, 0.3f);
    }

    void UpdatePath()
    {
        if (!seeker.IsDone()) return;

        switch (currentState)
        {
            case EnemyState.Chase:
                if (target != null)
                    seeker.StartPath(rb.position, target.position, OnPathComplete);
                break;

            case EnemyState.Patrol:
                if (patrolPoints.Count > 0)
                    seeker.StartPath(rb.position, patrolPoints[patrolIndex], OnPathComplete);
                break;
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
            case EnemyState.Patrol:
                HandlePatrol();
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

        MoveAlongPath();

        if (attackBehavior != null)
            attackBehavior.TryAttack(target, stats);
    }

    void HandlePatrol()
    {
        if (patrolPoints.Count == 0) return;

        if (isWaiting)
        {
            waitTimer -= Time.fixedDeltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
                patrolIndex = (patrolIndex + 1) % patrolPoints.Count;
                UpdatePath();
            }
            return;
        }

        if (path == null || currentWaypoint >= path.vectorPath.Count)
        {
            rb.linearVelocity = Vector2.zero;
            isWaiting = true;
            waitTimer = waitTimeAtPoint;
            return;
        }

        MoveAlongPath();
    }

    void MoveAlongPath()
    {
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        currentDirection = Vector2.Lerp(currentDirection, direction, 0.2f);
        Vector2 velocity = currentDirection * Mathf.Clamp(stats.moveSpeed, 0.5f, 3f);  // Clamp to avoid too fast
        rb.linearVelocity = velocity;

        if (stats.flipSprite && spriteRenderer != null)
        {
            spriteRenderer.flipX = currentDirection.x < -0.05f;
        }

        if (Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]) < stats.nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    void CheckPlayerProximity()
    {
        if (target == null) return;

        float distance = Vector2.Distance(rb.position, target.position);
        if (distance <= stats.stopDistance + 1f)
        {
            currentState = EnemyState.Chase;
        }
    }

    public void SetPatrolPoints(List<Vector3> points)
    {
        patrolPoints = points;
        patrolIndex = 0;
    }
}
