using UnityEngine;
using System.Collections;

[System.Serializable]
public class SpawnEntry
{
    public EnemyType type;
    public GameObject prefab;
}

public class EnemySpawner : MonoBehaviour
{
    public SpawnEntry[] enemyTypes;
    public Transform[] spawnPoints;
    public float spawnInterval = 5f;
    public int enemiesPerWave = 1;
    public float triggerDistance = 5f; // Distance from player to trigger spawn
    public bool spawnOnce = true;

    private bool hasSpawned = false;
    private bool isSpawning = false;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null || (hasSpawned && spawnOnce)) return;

        foreach (var point in spawnPoints)
        {
            float distance = Vector2.Distance(player.position, point.position);
            if (distance <= triggerDistance && !isSpawning)
            {
                StartCoroutine(SpawnWaveRepeatedly());
                if (spawnOnce) hasSpawned = true;
                break;
            }
        }
    }

    IEnumerator SpawnWaveRepeatedly()
    {
        isSpawning = true;

        do
        {
            for (int i = 0; i < enemiesPerWave; i++)
            {
                var entry = enemyTypes[Random.Range(0, enemyTypes.Length)];
                var point = spawnPoints[Random.Range(0, spawnPoints.Length)];

                GameObject enemy = Instantiate(entry.prefab, point.position, Quaternion.identity);

                var ai = enemy.GetComponent<EnemyAI>();
                if (ai != null)
                {
                    switch (entry.type)
                    {
                        case EnemyType.Idle: ai.currentState = EnemyState.Idle; break;
                        case EnemyType.Patrol: ai.currentState = EnemyState.Patrol; break;
                        case EnemyType.Chase: ai.currentState = EnemyState.Chase; break;
                        default: ai.currentState = EnemyState.Idle; break;
                    }
                }
            }

            yield return new WaitForSeconds(spawnInterval);

        } while (!spawnOnce);

        isSpawning = false;
    }
}
