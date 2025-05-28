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
    [Header("References")]
    public TileMapVisualizer tileMapVisualizer;
    public Transform player;

    [Header("Spawn Settings")]
    public SpawnEntry[] enemyTypes;
    public int spawnPointCount = 10;
    public int enemiesPerSpawn = 2;
    public float spawnOffset = 0.5f;
    public float triggerRadius = 7f;
    public float spawnInterval = 3f;
    public float minDistanceFromPlayer = 6f;
    public float minDistanceBetweenSpawners = 5f;

    private List<Vector3> spawnPositions = new List<Vector3>();
    private Dictionary<Vector3, Coroutine> activeSpawns = new Dictionary<Vector3, Coroutine>();

    void Start()
    {
        if (tileMapVisualizer == null || player == null || enemyTypes.Length == 0)
        {
            Debug.LogError("[EnemySpawner] Missing references.");
            return;
        }

        List<Vector2Int> floorTiles = tileMapVisualizer.GetFloorWorldPositions();

        int attempts = 0;
        while (spawnPositions.Count < spawnPointCount && attempts < 1000)
        {
            attempts++;
            Vector2Int tile = floorTiles[Random.Range(0, floorTiles.Count)];
            Vector3 spawnPos = new Vector3(tile.x + spawnOffset, tile.y + spawnOffset, 0f);

            bool tooClose = false;

            // Don't place near player
            if (Vector3.Distance(spawnPos, player.position) < minDistanceFromPlayer)
            {
                tooClose = true;
            }

            // Don't place near other spawn points
            if (!tooClose)
            {
                foreach (var existing in spawnPositions)
                {
                    if (Vector3.Distance(spawnPos, existing) < minDistanceBetweenSpawners)
                    {
                        tooClose = true;
                        break;
                    }
                }
            }

            if (!tooClose)
            {
                spawnPositions.Add(spawnPos);
            }
        }

        Debug.Log($"[EnemySpawner] Placed {spawnPositions.Count} spawn points after {attempts} attempts.");
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

                    // Assign player to AI if needed
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
