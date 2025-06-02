using Unity.VisualScripting;
using UnityEngine;

public abstract class JapanAbstractDungeonGenerator : MonoBehaviour
{
    [SerializeField] protected JapanTileMapVisualizer tileMapVisualizer = null;
    [SerializeField] protected Vector2Int startPosition = Vector2Int.zero;
    [SerializeField] protected ItemManager itemManager = null;
    [SerializeField] protected ItemManager itemManagerRoad = null;
    [SerializeField] protected bool spawnItems = true;
    [SerializeField] protected int minLootRange, maxLootRange;

    public void generateDungeon()
    {
        itemManager.Clear();
        itemManagerRoad.Clear();
        tileMapVisualizer.Clear();
        RunProceduralGeneration();
    }

    protected abstract void RunProceduralGeneration();

}

