using System;
using System.Collections.Generic;
using UnityEngine;
using static ProceduralGeneration;

public static class WallGenerator
{
    public static void CreateWalls(HashSet<Vector2Int> floorPositions, TileMapVisualizer tilemapVisualizer)
    {
        var basicWallPositions = FindWallsInDirections(floorPositions, Direction2D.cardinalDirectionsList);
        var cornerWallPositions = FindWallsInDirections(floorPositions, Direction2D.diagonalDirectionsList);
        //var emptyWallPositions = FindEmptyWalls(floorPositions, basicWallPositions, cornerWallPositions)

        CreateCornerWalls(tilemapVisualizer, basicWallPositions, cornerWallPositions, floorPositions);
        CreateBasicWalls(tilemapVisualizer, basicWallPositions, cornerWallPositions, floorPositions);
    }

    private static void CreateCornerWalls(TileMapVisualizer tilemapVisualizer, HashSet<Vector2Int> basicWallPositions, HashSet<Vector2Int> cornerWallPositions, HashSet<Vector2Int> floorPositions)
    {
        foreach (var position in cornerWallPositions)
        {
            string binaryType = "";
            string binaryWallType = "";
            foreach (var direction in Direction2D.eightDirectionsList)
            {
                var neighborPosition = position + direction;
                if (floorPositions.Contains(neighborPosition))
                {
                    binaryType += "1";
                }
                else
                {
                    binaryType += "0";
                }
            }
            foreach (var direction in Direction2D.cardinalDirectionsList)
            {
                var neighborPosition = position + direction;
                if (basicWallPositions.Contains(neighborPosition) || cornerWallPositions.Contains(neighborPosition))
                    binaryWallType += "1";
                else binaryWallType += "0";
            }
            tilemapVisualizer.PaintSingleCornerWall(position, binaryType, binaryWallType);
        }
    }

    private static void CreateBasicWalls(TileMapVisualizer tilemapVisualizer, HashSet<Vector2Int> basicWallPositions, HashSet<Vector2Int> cornerWallPositions, HashSet<Vector2Int> floorPositions)
    {
        foreach (var position in basicWallPositions)
        {
            string binaryType = "";
            string binaryWallType = "";
            foreach (var direction in Direction2D.cardinalDirectionsList)
            {
                var neighborPosition = position + direction;
                if (floorPositions.Contains(neighborPosition))
                    binaryType += "1";
                else binaryType += "0";

                if (basicWallPositions.Contains(neighborPosition) || cornerWallPositions.Contains(neighborPosition))
                    binaryWallType += "1";
                else binaryWallType += "0";
            }
            tilemapVisualizer.PaintSingleBasicWall(position, binaryType, binaryWallType);
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
