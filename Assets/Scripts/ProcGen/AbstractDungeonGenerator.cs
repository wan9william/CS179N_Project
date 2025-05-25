using Unity.VisualScripting;
using UnityEngine;

public abstract class AbstractDungeonGenerator : MonoBehaviour
{
    [SerializeField] protected TileMapVisualizer tileMapVisualizer = null;
    [SerializeField] protected Vector2Int startPosition = Vector2Int.zero;
    [SerializeField] protected ItemManager itemManager = null;
    [SerializeField] protected bool spawnItems = true;
    [SerializeField] protected float playerSpawnRange = 4f;
    [SerializeField] protected float maxLootRange = 50f;

    public void generateDungeon()
    {
        itemManager.Clear();
        tileMapVisualizer.Clear();
        RunProceduralGeneration();
    }

    protected abstract void RunProceduralGeneration();

}

