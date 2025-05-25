using UnityEngine;
using UnityEngine.Tilemaps;
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
    public SpawnEntry[] enemyTypes;
    public TileMapVisualizer tileMapVisualizer;
    public int enemiesToSpawn = 10;
    public float spawnInterval = 0.3f;
    public float spawnOffset = 0.5f;

    private BoundsInt mapBounds;
    private List<Vector3> floorWorldPositions = new List<Vector3>();

    void Start()
    {
        ExtractFloorPositions();
        StartCoroutine(SpawnEnemiesOnFloor());
    }

    void ExtractFloorPositions()
    {
        Tilemap floorTilemap = tileMapVisualizer.GetComponent<TileMapVisualizer>().GetComponentInChildren<Tilemap>();
        mapBounds = floorTilemap.cellBounds;

        foreach (var pos in mapBounds.allPositionsWithin)
        {
            if (floorTilemap.HasTile(pos))
            {
                Vector3 worldPos = floorTilemap.CellToWorld(pos) + new Vector3(spawnOffset, spawnOffset, 0);
                floorWorldPositions.Add(worldPos);
            }
        }

        Debug.Log($"[EnemySpawner] Found {floorWorldPositions.Count} floor tiles.");
    }

    IEnumerator SpawnEnemiesOnFloor()
    {
        HashSet<Vector3> used = new HashSet<Vector3>();

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            if (floorWorldPositions.Count == 0)
            {
                Debug.LogWarning("[EnemySpawner] No valid floor tiles found.");
                yield break;
            }

            Vector3 spawnPos;
            do
            {
                spawnPos = floorWorldPositions[Random.Range(0, floorWorldPositions.Count)];
            } while (used.Contains(spawnPos));
            used.Add(spawnPos);

            var entry = enemyTypes[Random.Range(0, enemyTypes.Length)];
            GameObject enemy = Instantiate(entry.prefab, spawnPos, Quaternion.identity);

            var ai = enemy.GetComponent<EnemyAI>();
            if (ai != null)
            {
                switch (entry.type)
                {
                    case EnemyType.Idle: ai.currentState = EnemyState.Idle; break;
                    case EnemyType.Patrol: ai.currentState = EnemyState.Patrol; break;
                    case EnemyType.Chase: ai.currentState = EnemyState.Chase; break;
                }
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
