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
    public TileMapVisualizer tileMapVisualizer;
    public SpawnEntry[] enemyTypes;
    public int enemiesPerSpawn = 2;
    public float spawnOffset = 0.5f;
    public float triggerRadius = 7f;
    public float spawnInterval = 3f;

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
            Debug.LogWarning("[EnemySpawner] No floor tiles found. Spawning aborted.");
            return;
        }

        for (int i = 0; i < 10; i++) // Set number of spawn points
        {
            Vector2Int tile = floorPositions[Random.Range(0, floorPositions.Count)];
            Vector3 spawnPos = new Vector3(tile.x + spawnOffset, tile.y + spawnOffset, 0);
            spawnPositions.Add(spawnPos);
        }

        Debug.Log($"[EnemySpawner] Prepared {spawnPositions.Count} spawn positions.");
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

    IEnumerator SpawnEnemiesAtPoint(Vector3 spawnPoint)
    {
        while (true)
        {
            for (int i = 0; i < enemiesPerSpawn; i++)
            {
                var entry = enemyTypes[Random.Range(0, enemyTypes.Length)];
                if (entry.prefab != null)
                {
                    Instantiate(entry.prefab, spawnPoint, Quaternion.identity);
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
