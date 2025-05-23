using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class CorridorFirstDungeonGenerator : DungeonGenerator
{
    //PCG Parameters
    [SerializeField] private int corridorLength = 14, corridorCount = 5;
    [SerializeField] [Range(0.1f,1f)] public float roomPercent = 0.8f;
    [SerializeField] private GameObject roomPrefab;
    [SerializeField] private int minimumRoomLength = 6;
    [SerializeField] private int maximumRoomLength = 17;
    [SerializeField] private int corridorSize = 3;
    [SerializeField] private bool randomWalk = false;

    //PCG Data
    private Dictionary<Vector2Int, HashSet<Vector2Int>> roomsDictionary = new Dictionary<Vector2Int, HashSet<Vector2Int>>();

    private HashSet<Vector2Int> floorPositions, corridorPositions;

    //Color
    private List<Color> roomColors = new List<Color>();

    
    protected override void RunProceduralGeneration()
    {
        CorridorFirstGeneration();
    }

    //Creates Corridors, then Rooms on the Corridors, then Walls.
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
            corridors[i] = IncreaseCorridorBrush(corridors[i],corridorSize);
            floorPositions.UnionWith(corridors[i]);
        }

        tileMapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions,tileMapVisualizer);
        SpawnItems(floorPositions);
    }

    private List<Vector2Int> IncreaseCorridorBrush(List<Vector2Int> corridor, int size)
    {
        List<Vector2Int> newCorridor = new List<Vector2Int>();
        for (int i = 0; i < corridor.Count; i++)
        {
            for(int x = -size/2; x < size/2;  x++)
            {
                for(int y = -size/2; y < size/2; y++)
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
            if (position == new Vector2Int(0, 0)) continue; //remove 
            if (roomFloors.Contains(position) == false)
            {
                var room = randomWalk ? RunRandomWalk(randomWalkParameters, position) : RunRectangleWalk(position, (int)UnityEngine.Random.Range(minimumRoomLength, maximumRoomLength), (int)UnityEngine.Random.Range(minimumRoomLength, maximumRoomLength));
                roomFloors.UnionWith(room);
            }
        }
    }

    private List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> floorPositions)
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>();
        foreach (var position in floorPositions)
        {
            int neighborsCount = 0;
            foreach (var direction in Direction2D.cardinalDirectionsList)
            {
                if(floorPositions.Contains(position + direction))
                    neighborsCount++;
            }

            if (neighborsCount == 1)
                deadEnds.Add(position);
        }
        return deadEnds;
    }
    
    private void ClearRoomData()
    {
        roomsDictionary.Clear();
        roomColors.Clear();
    }

    private void SaveRoomData(Vector2Int roomPosition, HashSet<Vector2Int> roomFloor)
    {
        roomsDictionary[roomPosition] = roomFloor;
        roomColors.Add(UnityEngine.Random.ColorHSV());
    }
    private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> potentialRoomPositions)
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        int roomToCreateCount = Mathf.RoundToInt(potentialRoomPositions.Count*roomPercent);



        List<Vector2Int> roomsToCreate = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomToCreateCount).ToList();
        var roomFloor = new HashSet<Vector2Int>();
        roomFloor = RunRectangleWalk(new Vector2Int(0, 0), 10, 10);
        roomPositions.UnionWith(roomFloor);

        foreach (var roomPosition in roomsToCreate)
        {
            if (roomPosition == new Vector2Int(0, 0)) continue;
            roomFloor = randomWalk ? RunRandomWalk(randomWalkParameters, roomPosition) : RunRectangleWalk(roomPosition, (int)UnityEngine.Random.Range(minimumRoomLength, maximumRoomLength), (int)UnityEngine.Random.Range(minimumRoomLength, maximumRoomLength));
            SaveRoomData(roomPosition, roomFloor);
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
