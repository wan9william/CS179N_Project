using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ProceduralGeneration;

public static class WallGenerator
{
    public static void CreateWalls(HashSet<Vector2Int> floorPositions, TileMapVisualizer tilemapVisualizer)
    {
        List< Vector2Int> directionList = new List<Vector2Int>();
        directionList = Direction2D.cardinalDirectionsList.Union(Direction2D.diagonalDirectionsList).ToList<Vector2Int>();
        var basicWallPositions = FindWallsInDirections(floorPositions, directionList);
        foreach (var position in basicWallPositions) 
        {
            var front = position + ProceduralGeneration.Direction2D.cardinalDirectionsList[2];
            tilemapVisualizer.PaintSingleBasicWall(position);
            if (!basicWallPositions.Contains(front))
                tilemapVisualizer.PaintSingleBasicWallFront(front);
        }
    }

    private static HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> floorPositions, List<Vector2Int> directionList)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        foreach (var position in floorPositions)
        {
            foreach (var direction in directionList)
            {
                var neighborPosition = position + direction;
                if (!floorPositions.Contains(neighborPosition))
                {
                    wallPositions.Add(neighborPosition);
                }
            }
        }
        return wallPositions;
    }
}
