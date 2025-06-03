using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class DungeonGenerator : AbstractDungeonGenerator
{

    [SerializeField] protected SimpleRandomWalkData randomWalkParameters;

    public override void RunProceduralGeneration()
    {
        tileMapVisualizer.Clear();
        HashSet<Vector2Int> floorPositions = RunRandomWalk(randomWalkParameters, startPosition);
        HashSet<Vector2Int> doorPositions = new HashSet<Vector2Int>();
        itemManager.Clear();
        tileMapVisualizer.Clear();
        tileMapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tileMapVisualizer);
        pathfinder.Scan();
        SpawnItems(floorPositions, doorPositions);
    }

    protected void SpawnItems(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> doorPositions)
    {
        foreach (var position in floorPositions)
        {
            double distance = Math.Sqrt(Math.Pow(position.x, 2) + Math.Pow(position.y, 2));
            if (doorPositions.Contains(position)) continue;
            string binaryType = "";
            foreach (var direction in Direction2D.eightDirectionsList)
            {
                var neighborPosition = position + direction;
                if (floorPositions.Contains(neighborPosition))
                    binaryType += "1";
                else binaryType += "0";
            }

            int typeAsInt = Convert.ToInt32(binaryType, 2);

            if (spawnItems && InSpawnArea(position,distance))
            {
                if (WallTypesHelper.floorEdge.Contains(typeAsInt))
                {
                    itemManager.InstantiateObject(new Vector3(position.x, position.y, 0), itemManager.transform);
                }
                else if (CheckEightDirections(position, floorPositions))
                {
                    if (Random.value < 0.2f)
                    {
                        itemManager.InstantiateObject(new Vector3(position.x, position.y, 0), itemManager.transform);
                    }
                    else
                    {
                        itemManager.InstantiateLoot(new Vector3(position.x, position.y, 0), distance, itemManager.transform);
                    }
                }
            }
        }
    }

    protected bool InSpawnArea(Vector2Int position, double distance)
    {
        return (distance >= minLootRange) && (distance <= maxLootRange);
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

    protected HashSet<Vector2Int> RunRectangleWalkTL(Vector2Int position, int width, int length)
    {
        var currentPositon = position;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        var path = ProceduralGeneration.SimpleRectangleBL(currentPositon, width, length);
        floorPositions.UnionWith(path);
        return floorPositions;
    }

}
