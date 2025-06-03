using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Seeker), typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    [Header("Enemy Stats")]
    public EnemyStats stats;
    public Transform target;
    public SpriteRenderer spriteRenderer;

    //[Header("Patrol Settings")]
    //public Transform[] patrolPoints;
    //private int currentPatrolIndex = 0;

    private Seeker seeker;
    private Rigidbody2D rb;
    private Path path;
    private Vector2 currentDirection;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath = false;

    private EnemyAttack attackBehavior;
    public EnemyState currentState = EnemyState.Idle;

    void Start()
    {
        target = GameObject.Find("Player").transform;
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        // ✅ Auto-assign Player target if not already set
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
            else
            {
                Debug.LogError("[EnemyAI] Player not found. Make sure the player has the 'Player' tag.");
            }
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

            //case EnemyState.Patrol:
            //    HandlePatrol();
            //    break;

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

        if (attackBehavior != null)
        {
            attackBehavior.TryAttack(target, stats);
        }

        if (Vector2.Distance(rb.position, target.position) <= stats.stopDistance)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 targetDirection = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        currentDirection = Vector2.Lerp(currentDirection, targetDirection, 0.2f);
        Vector2 nextPosition = rb.position + currentDirection * stats.moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(nextPosition);

        if (stats.flipSprite && spriteRenderer != null)
        {
            spriteRenderer.flipX = currentDirection.x < -0.05f;
        }

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < stats.nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    //void HandlePatrol()
    //{
    //    if (patrolPoints.Length == 0) return;

    //    Vector2 patrolTarget = patrolPoints[currentPatrolIndex].position;
    //    Vector2 dir = (patrolTarget - rb.position).normalized;
    //    rb.MovePosition(rb.position + dir * stats.moveSpeed * Time.fixedDeltaTime);

    //    if (Vector2.Distance(rb.position, patrolTarget) < 0.2f)
    //    {
    //        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    //    }

    //    if (stats.flipSprite && spriteRenderer != null)
    //    {
    //        spriteRenderer.flipX = dir.x < -0.05f;
    //    }
    //}

    void CheckPlayerProximity()
    {
        if (target == null) return;

        float distance = Vector2.Distance(rb.position, target.position);

        if (distance <= stats.stopDistance + 1f)
        {
            currentState = EnemyState.Chase;
        }
        //else if (currentState == EnemyState.Chase && distance > stats.stopDistance + 2f)
        //{
        //    currentState = EnemyState.Patrol;
        //}
    }
}
