using UnityEngine;
using System.Linq;


public class GameDirector : MonoBehaviour
{
    [Header("References")]
    public PlayerHealth playerHealth;
    public DayNightTimer timer;
    public ItemManager itemManager;
    public EnemySpawner enemySpawner;

    [Header("Difficulty Settings")]
    public int baseEnemiesPerSpawn = 2;
    public float baseItemChance = 1f;
    public int maxEnemiesPerSpawn = 6;
    public float minItemChance = 0.2f;

    [Header("Scaling Weights")]
    [Range(0f, 1f)] public float healthWeight = 0.4f;
    [Range(0f, 1f)] public float resourceWeight = 0.3f;
    [Range(0f, 1f)] public float timeWeight = 0.3f;

    private InventorySlot[] inventorySlots;


    void Start()
    {
        inventorySlots = Object.FindObjectsByType<InventorySlot>(FindObjectsSortMode.None);

    }

    void Update()
    {
        if (playerHealth == null || timer == null || itemManager == null || enemySpawner == null)
            return;

        float difficulty = CalculateDifficulty();

        // Adjust enemies per spawn
        enemySpawner.enemiesPerSpawn = Mathf.RoundToInt(Mathf.Lerp(baseEnemiesPerSpawn, maxEnemiesPerSpawn, difficulty));

        // Adjust item drop chance
        itemManager.itemChancePerTile = Mathf.Lerp(baseItemChance, minItemChance, difficulty);
    }

    float CalculateDifficulty()
    {
        float healthRatio = (float)playerHealth.GetCurrentHealth() / playerHealth.GetMaxHealth(); // 1 when full, 0 when dead
        float healthScore = 1f - healthRatio; // more difficult when low health

        int totalResources = 0;
        foreach (var slot in inventorySlots)
        {
            totalResources += slot.GetQuantity();
        }
        float resourceScore = Mathf.Clamp01((float)totalResources / 50f); // Adjust 50f based on max expected

        float timeRemaining = Mathf.Max(0f, timer.GetTimeRemaining());
        float timeRatio = timeRemaining / timer.GetTotalTime(); // 1 = early, 0 = end
        float timeScore = 1f - timeRatio; // more difficult as time runs out

        float difficulty =
            (healthScore * healthWeight) +
            (resourceScore * resourceWeight) +
            (timeScore * timeWeight);

        return Mathf.Clamp01(difficulty);
    }
}
