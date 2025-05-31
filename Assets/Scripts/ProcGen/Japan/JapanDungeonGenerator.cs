using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class JapanDungeonGenerator : JapanAbstractDungeonGenerator
{

    [SerializeField] protected SimpleRandomWalkData randomWalkParameters;

    protected override void RunProceduralGeneration()
    {
        HashSet<Vector2Int> floorPositions = RunRandomWalk(randomWalkParameters, startPosition);
        HashSet<Vector2Int> roadPositions = new HashSet<Vector2Int>();
        itemManager.Clear();
        tileMapVisualizer.Clear();
        tileMapVisualizer.PaintFloorTiles(floorPositions);
        JapanWallGenerator.CreateWalls(floorPositions, roadPositions, tileMapVisualizer);
        SpawnItems(floorPositions);
    }

    protected void SpawnItems(HashSet<Vector2Int> positions)
    {
        foreach (var position in positions)
        {
            if (spawnItems && InSpawnArea(position) && CheckEightDirections(position, positions)) itemManager.InstantiateLoot(new Vector3(position.x, position.y, 0), itemManager.transform);
        }
    }

    protected bool InSpawnArea(Vector2Int position)
    {
        return (Math.Abs(position.x) >= minLootRange || Math.Abs(position.y) >= minLootRange) && (Math.Abs(position.x) <= maxLootRange && Math.Abs(position.y) <= maxLootRange);
    }

    protected bool CheckEightDirections(Vector2Int position, IEnumerable<Vector2Int> floorPositions)
    {
        foreach (Vector2Int direction in Direction2D.eightDirectionsList)
        {
            if (!floorPositions.Contains(position + direction)) return false;
        }
        return true;
    }

    protected HashSet<Vector2Int> RunRandomWalk(SimpleRandomWalkData parameters, Vector2Int position)
    {
        var currentPositon = position;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        for (int i = 0; i < parameters.iterations; i++)
        {
            var path = ProceduralGeneration.SimpleRandomWalk(currentPositon, parameters.walkLength);
            floorPositions.UnionWith(path);
            if (parameters.startRandomlyEachIteration) 
            {
                currentPositon = floorPositions.ElementAt(Random.Range(0,floorPositions.Count));

            }
        }
        return floorPositions;
    }

    protected HashSet<Vector2Int> RunRectangleWalk(Vector2Int position, int width, int length)
    {
        var currentPositon = position;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        var path = ProceduralGeneration.SimpleRectangle(currentPositon, width, length);
        floorPositions.UnionWith(path);
        return floorPositions;
    }
    protected HashSet<Vector2Int> RunRectangleWalkBL(Vector2Int position, int width, int length)
    {
        var currentPositon = position;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        var path = ProceduralGeneration.SimpleRectangleBL(currentPositon, width, length);
        floorPositions.UnionWith(path);
        return floorPositions;
    }
    protected HashSet<Vector2Int> RunRectangleWalkTL(Vector2Int position, int width, int length)
    {
        var currentPositon = position;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        var path = ProceduralGeneration.SimpleRectangleTL(currentPositon, width, length);
        floorPositions.UnionWith(path);
        return floorPositions;
    }
}
