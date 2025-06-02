using System.Collections.Generic;
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
    [SerializeField] protected List<GameObject> doorList;

    public void generateDungeon()
    {
        itemManager.Clear();
        itemManagerRoad.Clear();
        tileMapVisualizer.Clear();
        foreach (GameObject door in doorList)
        {
            DestroyImmediate(door);
        }
        doorList.Clear();
        RunProceduralGeneration();
    }

    protected abstract void RunProceduralGeneration();

}

