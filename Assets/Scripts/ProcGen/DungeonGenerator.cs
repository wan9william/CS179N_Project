using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonGenerator : AbstractDungeonGenerator
{

    [SerializeField] protected SimpleRandomWalkData randomWalkParameters;

    protected override void RunProceduralGeneration()
    {
        HashSet<Vector2Int> floorPositions = RunRandomWalk(randomWalkParameters, startPosition);
        tileMapVisualizer.Clear();
        tileMapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tileMapVisualizer);
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

}
