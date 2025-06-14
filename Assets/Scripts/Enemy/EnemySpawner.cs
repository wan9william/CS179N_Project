using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

[System.Serializable]
public class SpawnEntry
{
    public EnemyType type;
    public GameObject prefab;
}

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    public TileMapVisualizer tileMapVisualizer;
    public Transform player;
    public Transform ship;

    [Header("Spawn Settings")]
    public SpawnEntry[] enemyTypes;
    public int spawnPointCount = 10;
    public int enemiesPerSpawn = 2;
    public float spawnOffset = 0.5f;
    public float triggerRadius = 7f;
    public float spawnInterval = 3f;
    public float minDistanceFromPlayer = 6f;
    public float minDistanceFromShip = 8f;
    public float minDistanceBetweenSpawners = 5f;

    [Header("Spawn Restrictions")]
    [Range(1f, 5f)]
    public float shipSafetyMultiplier = 1.5f;

    [SerializeField] protected List<Vector3> spawnPositions = new List<Vector3>();
    protected Dictionary<Vector3, Coroutine> activeSpawns = new Dictionary<Vector3, Coroutine>();

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (ship == null)
            ship = GameObject.FindGameObjectWithTag("Ship")?.transform;

        if (tileMapVisualizer == null || player == null || ship == null || enemyTypes.Length == 0)
        {
            Debug.LogError("[EnemySpawner] Missing references.");
            return;
        }

        GenerateSpawnPositions();
    }

    void GenerateSpawnPositions()
    {
        List<Vector2Int> floorTiles = tileMapVisualizer.GetFloorWorldPositions();
        int attempts = 0;

        while (spawnPositions.Count < spawnPointCount && attempts < 1000)
        {
            attempts++;
            Vector2Int tile = floorTiles[Random.Range(0, floorTiles.Count)];
            Vector3 spawnPos = new Vector3(tile.x + spawnOffset, tile.y + spawnOffset, 0f);

            if (IsValidSpawnPosition(spawnPos))
            {
                spawnPositions.Add(spawnPos);
            }
        }

        Debug.Log($"[EnemySpawner] Placed {spawnPositions.Count} spawn points after {attempts} attempts.");
    }

    bool IsValidSpawnPosition(Vector3 spawnPos)
    {
        var grid = AstarPath.active?.data?.gridGraph;
        if (grid == null)
            return false;

        // Convert world position to graph-space position
        var node = grid.GetNearest(spawnPos, NNConstraint.Default).node;

        // Check if the node is walkable and not null (i.e., inside graph)
        if (node == null || !node.Walkable)
            return false;

        // Too close to player
        if (Vector3.Distance(spawnPos, player.position) < minDistanceFromPlayer)
            return false;

        // Too close to ship (with multiplier)
        float shipSafeDistance = minDistanceFromShip * shipSafetyMultiplier;
        if (Vector3.Distance(spawnPos, ship.position) < shipSafeDistance)
            return false;

        // Too close to other spawn points
        foreach (var existing in spawnPositions)
        {
            if (Vector3.Distance(spawnPos, existing) < minDistanceBetweenSpawners)
                return false;
        }

        return true;
    }


    void Update()
    {
        foreach (Vector3 point in spawnPositions)
        {
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

    IEnumerator SpawnEnemiesAtPoint(Vector3 point)
    {
        while (true)
        {
            for (int i = 0; i < enemiesPerSpawn; i++)
            {
                var entry = enemyTypes[Random.Range(0, enemyTypes.Length)];
                if (entry.prefab != null)
                {
                    GameObject enemy = Instantiate(entry.prefab, point, Quaternion.identity);
                    EnemyAI ai = enemy.GetComponent<EnemyAI>();
                    if (ai != null && ai.target == null)
                        ai.target = player;
                }
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public List<Vector3> GetEnemySpawnPositions()
    {
        return spawnPositions;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        foreach (var pos in spawnPositions)
        {
            Gizmos.DrawSphere(pos, 0.2f);
        }
    }
}
