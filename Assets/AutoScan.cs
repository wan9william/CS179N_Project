using UnityEngine;
using Pathfinding;
using System.Collections.Generic;

public class AStarBootstrap : MonoBehaviour
{
    public static AStarBootstrap Instance { get; private set; }

    [Header("Grid Settings")]
    public int padding = 5;

    private List<TileMapVisualizer> tileMapVisualizers = new();
    private List<JapanTileMapVisualizer> japanTileMapVisualizers = new();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        FindVisualizersInScene();
        UpdateAndScanGrid();
    }

    /// <summary>
    /// Dynamically find all visualizers in the current scene.
    /// </summary>
    private void FindVisualizersInScene()
    {
        tileMapVisualizers.Clear();
        japanTileMapVisualizers.Clear();

        tileMapVisualizers.AddRange(Object.FindObjectsByType<TileMapVisualizer>(FindObjectsSortMode.None));
        japanTileMapVisualizers.AddRange(Object.FindObjectsByType<JapanTileMapVisualizer>(FindObjectsSortMode.None));

        Debug.Log($"[AStarBootstrap] Found {tileMapVisualizers.Count} TileMapVisualizer(s) and {japanTileMapVisualizers.Count} JapanTileMapVisualizer(s).");
    }


    public void Scan()
    {
        FindVisualizersInScene();
        UpdateAndScanGrid();
    }

    public void UpdateAndScanGrid()
    {
        GridGraph grid = AstarPath.active?.data?.gridGraph;

        if (grid == null)
        {
            Debug.LogError("[AStarBootstrap] No GridGraph found. Make sure AstarPath is initialized.");
            return;
        }

        List<Vector2Int> allPositions = new();

        foreach (var visualizer in tileMapVisualizers)
            allPositions.AddRange(visualizer.GetFloorWorldPositions());

        foreach (var visualizer in japanTileMapVisualizers)
            allPositions.AddRange(visualizer.GetFloorWorldPositions());

        if (allPositions.Count == 0)
        {
            Debug.LogWarning("[AStarBootstrap] No tile positions found to define grid bounds.");
            return;
        }

        // Calculate bounds
        int minX = int.MaxValue, minY = int.MaxValue;
        int maxX = int.MinValue, maxY = int.MinValue;

        foreach (Vector2Int pos in allPositions)
        {
            minX = Mathf.Min(minX, pos.x);
            minY = Mathf.Min(minY, pos.y);
            maxX = Mathf.Max(maxX, pos.x);
            maxY = Mathf.Max(maxY, pos.y);
        }

        // Apply padding
        minX -= padding;
        minY -= padding;
        maxX += padding;
        maxY += padding;

        int width = (maxX - minX) + 1;
        int height = (maxY - minY) + 1;
        Vector3 center = new(minX + width / 2f, minY + height / 2f, 0f);

        grid.center = center;
        grid.SetDimensions(width, height, grid.nodeSize);

        Debug.Log($"[AStarBootstrap] Grid resized to width={width}, height={height}, center={center}");
        AstarPath.active.Scan();
    }
}
