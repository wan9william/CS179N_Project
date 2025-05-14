using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorridorFirstDungeonGenerator : DungeonGenerator
{
    [SerializeField] private int corridorLength = 14, corridorCount = 5;
    [SerializeField] [Range(0.1f,1f)] public float roomPercent = 0.8f;
    [SerializeField] private GameObject roomPrefab;
    protected override void RunProceduralGeneration()
    {
        CorridorFirstGeneration();
    }

    private void CorridorFirstGeneration()
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        HashSet<Vector2Int> potentialRoomPositions = new HashSet<Vector2Int>();

        List<List<Vector2Int>> corridors = CreateCorridors(floorPositions, potentialRoomPositions);

        HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomPositions);

        List<Vector2Int> deadEnds = FindAllDeadEnds(floorPositions);

        CreateRoomsAtDeadEnd(deadEnds, roomPositions);

        floorPositions.UnionWith(roomPositions);

        for (int i = 0; i < corridors.Count; i++)
        {
            corridors[i] = IncreaseCorridorBrush(corridors[i]);
            floorPositions.UnionWith(corridors[i]);
        }

        tileMapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions,tileMapVisualizer); 
    }

    private List<Vector2Int> IncreaseCorridorBrush(List<Vector2Int> corridor)
    {
        List<Vector2Int> newCorridor = new List<Vector2Int>();
        for (int i = 0; i < corridor.Count; i++)
        {
            for(int x = -1; x < 2;  x++)
            {
                for(int y = -1; y < 2; y++)
                {
                    newCorridor.Add(corridor[i] + new Vector2Int(x,y));
                }
            }
        }
        return newCorridor;
    }

    private void CreateRoomsAtDeadEnd(List<Vector2Int> deadEnds, HashSet<Vector2Int> roomFloors)
    {
        foreach (var position in deadEnds)
        {
            //if (roomFloors.Contains(position) == false)
            //{
            //    var room = RunRandomWalk(randomWalkParameters, position);
            //    roomFloors.UnionWith(room);
            //}
            if (roomFloors.Contains(position) == false)
            {
                var room = RunRectangleWalk(position, (int)UnityEngine.Random.Range(6.0f, 17.0f), (int)UnityEngine.Random.Range(6.0f, 17.0f));
                roomFloors.UnionWith(room);
            }
            //Quaternion roomRotation = Quaternion.Euler(0f, 0f, 0f);
            //Vector3 roomPosition = Vector3.zero;
            //roomPosition.x = position.x;
            //roomPosition.y = position.y;
            //Instantiate(roomPrefab, roomPosition, Quaternion.identity);
        }
    }

    private List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> floorPositions)
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>();
        foreach (var position in floorPositions)
        {
            int neighborsCount = 0;
            foreach (var direction in ProceduralGeneration.Direction2D.cardinalDirectionsList)
            {
                if(floorPositions.Contains(position + direction))
                    neighborsCount++;
            }

            if (neighborsCount == 1)
                deadEnds.Add(position);
        }
        return deadEnds;
    }

    private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> potentialRoomPositions)
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        int roomToCreateCount = Mathf.RoundToInt(potentialRoomPositions.Count*roomPercent);

        List<Vector2Int> roomsToCreate = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomToCreateCount).ToList();

        foreach (var roomPosition in roomsToCreate) 
        {
            //var roomFloor = RunRandomWalk(randomWalkParameters, roomPosition);
            var roomFloor = RunRectangleWalk(roomPosition, (int)UnityEngine.Random.Range(6.0f, 17.0f), (int)UnityEngine.Random.Range(6.0f, 17.0f));
            roomPositions.UnionWith(roomFloor);
        }

        return roomPositions;
    }

    private List<List<Vector2Int>> CreateCorridors(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> potentialRoomPositions)
    {
        var currentPosition = startPosition;
        potentialRoomPositions.Add(startPosition);
        List<List<Vector2Int>> corridors = new List<List<Vector2Int>>();

        for (int i = 0; i < corridorCount; i++) 
        {
            var corridor = ProceduralGeneration.RandomWalkCorridors(currentPosition, corridorLength);
            currentPosition = corridor[corridor.Count - 1];
            potentialRoomPositions.Add((Vector2Int)currentPosition);
            floorPositions.UnionWith(corridor);
            corridors.Add(corridor);
        }
        return corridors;
    }
}
