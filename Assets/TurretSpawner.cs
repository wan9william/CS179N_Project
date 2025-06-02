using System.Collections.Generic;
using UnityEngine;

public class TurretSpawner : MonoBehaviour
{
    [Header("Dependencies")]
    public TileMapVisualizer tileMapVisualizer;
    public EnemySpawner enemySpawner;
    public GameObject turretPrefab;
    public Transform playerTransform;

    [Header("Spawn Settings")]
    public int turretsToSpawn = 3;
    public float spawnOffset = 0.5f;
    public float minDistanceFromEnemies = 5f;
    public float minDistanceFromPlayer = 6f;
    public float minDistanceBetweenTurrets = 6f;

    private List<Vector3> turretSpawnPositions = new List<Vector3>();

    void Start()
    {
        if (turretPrefab == null || tileMapVisualizer == null || enemySpawner == null || playerTransform == null)
        {
            Debug.LogError("[TurretSpawner] Missing required references!");
            return;
        }

        List<Vector2Int> floorTiles = tileMapVisualizer.GetFloorWorldPositions();
        List<Vector3> enemySpawnPositions = enemySpawner.GetEnemySpawnPositions();

        if (floorTiles.Count == 0)
        {
            Debug.LogWarning("[TurretSpawner] No floor tiles found. Turret spawning skipped.");
            return;
        }

        int attempts = 0;

        while (turretSpawnPositions.Count < turretsToSpawn && attempts < 1000)
        {
            attempts++;

            Vector2Int tile = floorTiles[Random.Range(0, floorTiles.Count)];
            Vector3 spawnPos = new Vector3(tile.x + spawnOffset, tile.y + spawnOffset, 0f);

            bool tooClose = false;

            // Check distance from enemy spawns
            foreach (var enemyPos in enemySpawnPositions)
            {
                if (Vector3.Distance(spawnPos, enemyPos) < minDistanceFromEnemies)
                {
                    tooClose = true;
                    break;
                }
            }

            // Check distance from player
            if (!tooClose && Vector3.Distance(spawnPos, playerTransform.position) < minDistanceFromPlayer)
            {
                tooClose = true;
            }

            // Check distance from other turrets
            if (!tooClose)
            {
                foreach (var turretPos in turretSpawnPositions)
                {
                    if (Vector3.Distance(spawnPos, turretPos) < minDistanceBetweenTurrets)
                    {
                        tooClose = true;
                        break;
                    }
                }
            }

            if (!tooClose)
            {
                GameObject turret = Instantiate(turretPrefab, spawnPos, Quaternion.identity);

                TurretBehavior turretBehavior = turret.GetComponent<TurretBehavior>();
                if (turretBehavior != null)
                {
                    turretBehavior.player = playerTransform;
                }

                turretSpawnPositions.Add(spawnPos);
            }
        }

        Debug.Log($"[TurretSpawner] Spawned {turretSpawnPositions.Count} turrets after {attempts} attempts.");
    }
}
