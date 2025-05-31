using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SpawnEntry
{
    public EnemyType type;
    public GameObject prefab;
}

public class EnemySpawner : MonoBehaviour
{
    [Header("Dependencies")]
    public TileMapVisualizer tileMapVisualizer;
    public SpawnEntry[] enemyTypes;
    public int spawnPointCount = 10;
    public int enemiesPerSpawn = 2;
    public float spawnOffset = 0.5f;
    public float triggerRadius = 7f;
    public float spawnInterval = 3f;
    public int patrolEnemyCount = 4;
    public float patrolRadius = 3f;
    public float minDistanceFromPlayer = 6f;
    public float minDistanceBetweenSpawns = 5f;

    [Header("Environment Check")]
    public LayerMask wallLayerMask;
    public float patrolPointCheckRadius = 0.2f;
    public int maxPatrolAttempts = 10;
    public float patrolSpawnClearRadius = 2f;
    public int requiredClearSpots = 5;

    private Transform player;
    private List<Vector3> spawnPositions = new List<Vector3>();
    private Dictionary<Vector3, Coroutine> activeSpawns = new Dictionary<Vector3, Coroutine>();

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null || tileMapVisualizer == null || enemyTypes.Length == 0)
        {
            Debug.LogError("[EnemySpawner] Missing required references.");
            return;
        }

        List<Vector2Int> floorPositions = tileMapVisualizer.GetFloorWorldPositions();
        if (floorPositions.Count == 0)
        {
            Debug.LogWarning("[EnemySpawner] No floor tiles found.");
            return;
        }

        int attempts = 0;
        while (spawnPositions.Count < spawnPointCount && attempts < 1000)
        {
            attempts++;
            Vector2Int tile = floorPositions[Random.Range(0, floorPositions.Count)];
            Vector3 spawnPos = new Vector3(tile.x + spawnOffset, tile.y + spawnOffset, 0);

            if (Vector3.Distance(player.position, spawnPos) < minDistanceFromPlayer)
                continue;

            bool tooClose = false;
            foreach (var pos in spawnPositions)
            {
                if (Vector3.Distance(pos, spawnPos) < minDistanceBetweenSpawns)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
            {
                spawnPositions.Add(spawnPos);
            }
        }

        int patrolSpawned = 0;
        foreach (var pos in spawnPositions)
        {
            if (patrolSpawned >= patrolEnemyCount) break;

            if (IsSpawnAreaOpen(pos))
            {
                SpawnEnemy(pos, true);
                patrolSpawned++;
            }
        }

        Debug.Log($"[EnemySpawner] Prepared {spawnPositions.Count} spawn points.");
    }

    void Update()
    {
        for (int i = patrolEnemyCount; i < spawnPositions.Count; i++)
        {
            Vector3 point = spawnPositions[i];
            float distance = Vector3.Distance(player.position, point);

            if (distance <= triggerRadius)
            {
                if (!activeSpawns.ContainsKey(point))
                {
                    Coroutine routine = StartCoroutine(SpawnEnemiesAtPoint(point));
                    activeSpawns[point] = routine;
                }
            }
            else
            {
                if (activeSpawns.ContainsKey(point))
                {
                    StopCoroutine(activeSpawns[point]);
                    activeSpawns.Remove(point);
                }
            }
        }
    }

    IEnumerator SpawnEnemiesAtPoint(Vector3 spawnPoint)
    {
        while (true)
        {
            for (int i = 0; i < enemiesPerSpawn; i++)
            {
                SpawnEnemy(spawnPoint, false);
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemy(Vector3 position, bool isPatrol)
    {
        var entry = enemyTypes[Random.Range(0, enemyTypes.Length)];
        if (entry.prefab != null)
        {
            GameObject enemy = Instantiate(entry.prefab, position, Quaternion.identity);
            if (enemy.TryGetComponent<EnemyAI>(out var ai))
            {
                ai.target = player;
                if (isPatrol)
                {
                    ai.currentState = EnemyState.Patrol;
                    ai.SetPatrolPoints(GenerateLocalPatrolPoints(position));
                }
            }
        }
    }

    bool IsSpawnAreaOpen(Vector3 position)
    {
        int openCount = 0;
        Vector2[] directions = {
            Vector2.up, Vector2.down, Vector2.left, Vector2.right,
            new Vector2(1, 1).normalized, new Vector2(-1, 1).normalized,
            new Vector2(1, -1).normalized, new Vector2(-1, -1).normalized
        };

        foreach (var dir in directions)
        {
            Vector2 checkPos = (Vector2)position + dir * patrolSpawnClearRadius;
            if (!Physics2D.OverlapCircle(checkPos, 0.3f, wallLayerMask))
            {
                openCount++;
            }
        }

        return openCount >= requiredClearSpots;
    }

    List<Vector3> GenerateLocalPatrolPoints(Vector3 center)
    {
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < 3; i++)
        {
            bool pointFound = false;
            for (int attempt = 0; attempt < maxPatrolAttempts; attempt++)
            {
                Vector2 offset = Random.insideUnitCircle * patrolRadius;
                Vector3 candidate = center + new Vector3(offset.x, offset.y, 0f);

                if (!Physics2D.OverlapCircle(candidate, patrolPointCheckRadius, wallLayerMask))
                {
                    points.Add(candidate);
                    pointFound = true;
                    break;
                }
            }

            if (!pointFound)
            {
                points.Add(center);
            }
        }
        return points;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        foreach (var pos in spawnPositions)
        {
            Gizmos.DrawWireSphere(pos, 0.3f);
            Gizmos.DrawWireSphere(pos, patrolSpawnClearRadius);
        }
    }

    public List<Vector3> GetEnemySpawnPositions()
    {
        return spawnPositions;
    }
}
