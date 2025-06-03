using Unity.VisualScripting;
using UnityEngine;

public abstract class AbstractDungeonGenerator : MonoBehaviour
{
    [SerializeField] protected TileMapVisualizer tileMapVisualizer = null;
    [SerializeField] protected AStarBootstrap pathfinder = null;
    [SerializeField] protected Vector2Int startPosition = Vector2Int.zero;
    [SerializeField] protected ItemManager itemManager = null;
    [SerializeField] protected bool spawnItems = true;
    [SerializeField] protected int minLootRange, maxLootRange;

    public void generateDungeon()
    {
        itemManager.Clear();
        tileMapVisualizer.Clear();
        RunProceduralGeneration();
        pathfinder.Scan();
    }

    protected abstract void RunProceduralGeneration();

}

