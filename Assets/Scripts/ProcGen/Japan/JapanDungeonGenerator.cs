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
        HashSet<Vector2Int> doorPositions = new HashSet<Vector2Int>();
        foreach (GameObject door in doorList)
        {
            DestroyImmediate(door);
        }
        doorList.Clear();
        itemManager.Clear();
        itemManagerRoad.Clear();
        tileMapVisualizer.Clear();
        tileMapVisualizer.PaintFloorTiles(floorPositions);
        JapanWallGenerator.CreateWalls(floorPositions, roadPositions, tileMapVisualizer);
        SpawnItems(floorPositions, roadPositions, doorPositions);
    }

    protected void SpawnItems(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> roadPositions, HashSet<Vector2Int> doorPositions)
    {
        foreach (var position in floorPositions)
        {
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

            if(spawnItems && InSpawnArea(position))
            {
                if (WallTypesHelper.floorEdge.Contains(typeAsInt))
                {
                    itemManager.InstantiateObject(new Vector3(position.x, position.y, 0), itemManager.transform);
                }
                else if (CheckEightDirections(position,floorPositions))
                {
                    if (Random.value < 0.2f)
                    {
                        itemManager.InstantiateObject(new Vector3(position.x, position.y, 0), itemManager.transform);
                    }
                    else
                    {
                        itemManager.InstantiateLoot(new Vector3(position.x, position.y, 0), itemManager.transform);
                    }
                }
            }
        }

        foreach (var position in roadPositions)
        {
            string binaryType = "";
            foreach (var direction in Direction2D.eightDirectionsList)
            {
                var neighborPosition = position + direction;
                if (roadPositions.Contains(neighborPosition))
                    binaryType += "1";
                else binaryType += "0";
            }
            int typeAsInt = Convert.ToInt32(binaryType, 2);
            if (spawnItems && InSpawnArea(position))
            {
                if (WallTypesHelper.floorEdgeHorizontal.Contains(typeAsInt))
                {
                    itemManagerRoad.InstantiateObject(new Vector3(position.x, position.y, 0), itemManagerRoad.transform);
                }
                else if (WallTypesHelper.floorAll.Contains(typeAsInt))
                {
                    itemManagerRoad.InstantiateLoot(new Vector3(position.x, position.y, 0), itemManagerRoad.transform);
                }
            }
        }
    }


    //TODO: CHECKS FOR EDGES OF ROOMS
    private bool isEdge(Vector2Int position, HashSet<Vector2Int> floorPositions)
    {
        string binaryType = "";
        foreach (var direction in Direction2D.eightDirectionsList)
        {
            var neighborPosition = position + direction;
            if (floorPositions.Contains(neighborPosition))
                binaryType += "1";
            else binaryType += "0";
        }

        int typeAsInt = Convert.ToInt32(binaryType, 2);

        return (WallTypesHelper.floorEdgeHorizontal.Contains(typeAsInt));
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
