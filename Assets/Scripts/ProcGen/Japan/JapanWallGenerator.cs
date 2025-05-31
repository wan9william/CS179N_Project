using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static ProceduralGeneration;

public static class JapanWallGenerator
{
    public static HashSet<Vector2Int> CreateWalls(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> roadPositions, JapanTileMapVisualizer tilemapVisualizer)
    {
        var basicWallPositions = FindWallsInDirections(floorPositions, roadPositions, Direction2D.cardinalDirectionsList);
        var cornerWallPositions = FindWallsInDirections(floorPositions, roadPositions, Direction2D.diagonalDirectionsList);

        CreateCornerWalls(tilemapVisualizer, basicWallPositions, cornerWallPositions, floorPositions);
        CreateBasicWalls(tilemapVisualizer, basicWallPositions, cornerWallPositions, floorPositions);

        basicWallPositions.UnionWith(cornerWallPositions);
        return basicWallPositions;
    }
    public static void CreateFences(HashSet<Vector2Int> roadPositions, HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> wallPositions, JapanTileMapVisualizer tilemapVisualizer)
    {
        var basicFencePositions = FindFencesInDirections(roadPositions, floorPositions, wallPositions, Direction2D.eightDirectionsList);

        CreateBasicFences(tilemapVisualizer, basicFencePositions);
    }

    private static void CreateBasicFences(JapanTileMapVisualizer tilemapVisualizer, HashSet<Vector2Int> basicFencePositions)
    {
        foreach (var position in basicFencePositions)
        {
            tilemapVisualizer.PaintSingleBasicFence(position);
        }
    }

    private static void CreateCornerWalls(JapanTileMapVisualizer tilemapVisualizer, HashSet<Vector2Int> basicWallPositions, HashSet<Vector2Int> cornerWallPositions, HashSet<Vector2Int> floorPositions)
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

    private static void CreateBasicWalls(JapanTileMapVisualizer tilemapVisualizer, HashSet<Vector2Int> basicWallPositions, HashSet<Vector2Int> cornerWallPositions, HashSet<Vector2Int> floorPositions)
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

    private static HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> roadPositions, List<Vector2Int> directionList)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        foreach (var position in floorPositions)
        {
            foreach (var direction in directionList)
            {
                var neighborPosition = position + direction;
                if (!floorPositions.Contains(neighborPosition) && !roadPositions.Contains(neighborPosition) && !roadPositions.Contains(neighborPosition + Direction2D.cardinalDirectionsList[2]))
                {
                    wallPositions.Add(neighborPosition);
                }
            }
        }
        return wallPositions;
    }

    private static HashSet<Vector2Int> FindFencesInDirections(HashSet<Vector2Int> roadPositions, HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> wallPositions, List<Vector2Int> directionList)
    {
        int minX = 0, minY = 0, maxX = 0, maxY = 0;
        HashSet<Vector2Int> fencePositions = new HashSet<Vector2Int>();
        foreach (var position in roadPositions)
        {
            minX = Math.Min(minX, position.x);
            minY = Math.Min(minY, position.y);
            maxX = Math.Max(maxX, position.x);
            maxY = Math.Max(maxY, position.y);

        }

        for(int x = minX-1; x < maxX+2; x++)
        {
            var neighborPositionMin = new Vector2Int(x, minY-1);
            var neighborPositionMax = new Vector2Int(x, maxY+1);
            if (!wallPositions.Contains(neighborPositionMin) && !roadPositions.Contains(neighborPositionMin) && !floorPositions.Contains(neighborPositionMin) && !wallPositions.Contains(neighborPositionMin + Direction2D.cardinalDirectionsList[0]))
            {
                fencePositions.Add(neighborPositionMin);
            }
            if (!wallPositions.Contains(neighborPositionMax) && !roadPositions.Contains(neighborPositionMax) && !floorPositions.Contains(neighborPositionMax) && !wallPositions.Contains(neighborPositionMax + Direction2D.cardinalDirectionsList[0]))
            {
                fencePositions.Add(neighborPositionMax);
            }
        }

        for (int y = minY-1; y < maxY+2; y++)
        {
            var neighborPositionMin = new Vector2Int(minX-1, y);
            var neighborPositionMax = new Vector2Int(maxX+1, y);
            if (!wallPositions.Contains(neighborPositionMin) && !roadPositions.Contains(neighborPositionMin) && !floorPositions.Contains(neighborPositionMin) && !wallPositions.Contains(neighborPositionMin + Direction2D.cardinalDirectionsList[0]))
            {
                fencePositions.Add(neighborPositionMin);
            }
            if (!wallPositions.Contains(neighborPositionMax) && !roadPositions.Contains(neighborPositionMax) && !floorPositions.Contains(neighborPositionMax) && !wallPositions.Contains(neighborPositionMax + Direction2D.cardinalDirectionsList[0]))
            {
                fencePositions.Add(neighborPositionMax);
            }
        }
        return fencePositions;
    }
}
