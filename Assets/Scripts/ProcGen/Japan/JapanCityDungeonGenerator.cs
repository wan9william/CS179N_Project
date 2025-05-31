using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class JapanCityDungeonGenerator : JapanDungeonGenerator
{
    //PCG Parameters
    [SerializeField] private int corridorLength = 14, corridorCount = 5;
    [SerializeField] [Range(0.1f,1f)] public float roomPercent = 0.8f;
    [SerializeField] private GameObject roomPrefab;
    
    
    [SerializeField] private int minimumHouseLength = 6, maximumHouseLength = 17;
    [SerializeField] private int initialAreaLength, initialAreaWidth = 10;
    [SerializeField] private int minRoomWidth = 4, minRoomHeight = 4;
    [SerializeField][Range(0, 10)] private int offset = 1;

    [SerializeField] private int corridorSize = 3;
    [SerializeField] private int roomOffset = 5;
    [SerializeField] private bool randomWalk = false;
   
    [SerializeField] int grassWidth, grassHeight = 0;



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
        HashSet<Vector2Int> roadPositions = new HashSet<Vector2Int>();
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();

        HashSet<Vector2Int> potentialRoomPositions = new HashSet<Vector2Int>();

        List<List<Vector2Int>> corridors = CreateCorridors(roadPositions, potentialRoomPositions);

        HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomPositions);

        //List<Vector2Int> deadEnds = FindAllDeadEnds(roadPositions);

        //CreateRoomsAtDeadEnd(deadEnds, roomPositions);

        floorPositions.UnionWith(roadPositions);
        floorPositions.UnionWith(roomPositions);

        tileMapVisualizer.PaintRoadTiles(roadPositions);
        tileMapVisualizer.PaintRoadIntersectionTiles(roadPositions);

        for (int i = 0; i < corridors.Count; i++)
        {
            corridors[i] = IncreaseCorridorBrush(corridors[i], corridorSize);
            roadPositions.UnionWith(corridors[i]);
        }

        tileMapVisualizer.PaintFloorTiles(roomPositions);
        tileMapVisualizer.PaintGrassTiles(grassWidth, grassHeight);
        var wallPositions = JapanWallGenerator.CreateWalls(roomPositions, roadPositions, tileMapVisualizer);
        JapanWallGenerator.CreateFences(roadPositions, floorPositions, wallPositions, tileMapVisualizer);
        SpawnItems(floorPositions);
    }

    private List<Vector2Int> IncreaseCorridorBrush(List<Vector2Int> corridor, int size)
    {
        List<Vector2Int> newCorridor = new List<Vector2Int>();
        for (int i = 0; i < corridor.Count; i++)
        {
            for(int x = -size; x < size+1;  x++)
            {
                for(int y = -size; y < size+1; y++)
                {
                    newCorridor.Add(corridor[i] + new Vector2Int(x,y));
                }
            }
        }
        return newCorridor;
    }

    //private void CreateRoomsAtDeadEnd(List<Vector2Int> deadEnds, HashSet<Vector2Int> roomFloors)
    //{
    //    Vector2Int roomOffset2D = new Vector2Int(roomOffset-1, roomOffset);
    //    foreach (var position in deadEnds)
    //    {
    //        if (position == new Vector2Int(0, 0)) continue; //remove first room
    //        if (roomFloors.Contains(position) == false)
    //        {
    //            var room = randomWalk ? RunRandomWalk(randomWalkParameters, position) : RunRectangleWalkBL(position + roomOffset2D, (int)UnityEngine.Random.Range(minimumRoomLength, maximumRoomLength), (int)UnityEngine.Random.Range(minimumRoomLength, maximumRoomLength));
    //            roomFloors.UnionWith(room);
    //        }
    //    }
    //}

    //private List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> floorPositions)
    //{
    //    List<Vector2Int> deadEnds = new List<Vector2Int>();
    //    foreach (var position in floorPositions)
    //    {
    //        int neighborsCount = 0;
    //        foreach (var direction in Direction2D.cardinalDirectionsList)
    //        {
    //            if(floorPositions.Contains(position + direction))
    //                neighborsCount++;
    //        }

    //        if (neighborsCount == 1)
    //            deadEnds.Add(position);
    //    }
    //    return deadEnds;
    //}
    
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
        roomFloor = RunRectangleWalkTL(new Vector2Int(roomOffset-2,roomOffset-2), initialAreaWidth, initialAreaLength);
        roomPositions.UnionWith(roomFloor);

        Vector2Int roomOffset2D = new Vector2Int(roomOffset-1, roomOffset);
        foreach (var roomPosition in roomsToCreate)
        {
            if (roomPosition == new Vector2Int(0, 0)) continue;

            roomFloor = CreateHouse(roomPosition + roomOffset2D);
            SaveRoomData(roomPosition+roomOffset2D, roomFloor);
            roomPositions.UnionWith(roomFloor);
        }
        return roomPositions;
    }


    private List<List<Vector2Int>> CreateCorridors(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> potentialRoomPositions)
    {
        var currentPosition = startPosition;
        potentialRoomPositions.Add(startPosition);
        List<List<Vector2Int>> corridors = new List<List<Vector2Int>>();

        //Add Initial Corridors Surrounding Initial Area
        foreach (var direction in Direction2D.cardinalDirectionsList)
        {
            List<Vector2Int> corridor = new List<Vector2Int>();
            for (int j = 1; j <= initialAreaLength; ++j)
            {
                corridor.Add(currentPosition + direction * j);

            }
            corridors.Add(corridor);
            floorPositions.UnionWith(corridor);
        }

        for (int i = 0; i < corridorCount; i++) 
        {
            var corridor = ProceduralGeneration.RandomWalkCorridors(currentPosition, corridorLength);
            currentPosition = corridor[corridor.Count - 1];
            potentialRoomPositions.Add((Vector2Int)currentPosition);
            List<Vector2Int> newcorridor = new List<Vector2Int>();
            for (int j = 1; j < 12; j++)
            {
                ;
                foreach(var direction in Direction2D.cardinalDirectionsList)
                {
                    newcorridor.Add(currentPosition+direction*j);
                }

            }
            corridors.Add(newcorridor);
            floorPositions.UnionWith(corridor);
            floorPositions.UnionWith(newcorridor);
            corridors.Add(corridor);
        }
        return corridors;
    }


    private HashSet<Vector2Int> CreateHouse(Vector2Int roomPosition)
    {
        var roomsList = ProceduralGeneration.BinarySpacePartitioning(new BoundsInt((Vector3Int)roomPosition, new Vector3Int(maximumHouseLength, maximumHouseLength, 0)), minRoomWidth, minRoomHeight);

        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

            floor = CreateSimpleRooms(roomsList);


        List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach (var room in roomsList)
        {
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }

        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);

        foreach(Vector2Int corridor in corridors)
        {
            floor.Add(corridor + Direction2D.cardinalDirectionsList[1]);
            floor.Add(corridor + Direction2D.cardinalDirectionsList[2]);
            floor.Add(corridor + Direction2D.diagonalDirectionsList[2]);
        }

        floor.UnionWith(corridors);
        return floor;
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        var currentRoomCenter = roomCenters[UnityEngine.Random.Range(0, roomCenters.Count)];
        roomCenters.Remove(currentRoomCenter);

        while (roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor);
        }
        return corridors;
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;
        corridor.Add(position);
        while (position.y != destination.y)
        {
            if (destination.y > position.y)
            {
                position += Vector2Int.up;
            }
            else if (destination.y < position.y)
            {
                position += Vector2Int.down;
            }
            corridor.Add(position);
        }
        while (position.x != destination.x)
        {
            if (destination.x > position.x)
            {
                position += Vector2Int.right;
            }
            else if (destination.x < position.x)
            {
                position += Vector2Int.left;
            }
            corridor.Add(position);
        }
        return corridor;
    }

    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;
        foreach (var position in roomCenters)
        {
            float currentDistance = Vector2.Distance(position, currentRoomCenter);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = position;
            }
        }
        return closest;
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        foreach (var room in roomsList)
        {
            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row = offset; row < room.size.y - offset; row++)
                {
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
                    floor.Add(position);
                }
            }
        }
        return floor;
    }

}
