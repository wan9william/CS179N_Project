using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI.Table;

public class JapanCityDungeonGenerator : JapanDungeonGenerator
{
    //PCG Parameters
    [SerializeField] private int corridorLength = 14, corridorCount = 5;
    [SerializeField] [Range(0.1f,1f)] public float roomPercent = 0.8f;
    
    [SerializeField] private int minimumHouseLength = 6, maximumHouseLength = 17;
    [SerializeField] private int initialAreaLength, initialAreaWidth = 10;
    [SerializeField] private int minRoomWidth = 4, minRoomHeight = 4;
    [SerializeField][Range(0, 10)] private int offset = 1;

    [SerializeField] private int corridorSize = 3;
    [SerializeField] private int roomOffset = 5;

    [SerializeField] private GameObject doorHorizontalPrefab;
    [SerializeField] private GameObject doorVerticalPrefab;

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
        HashSet<Vector2Int> doorPositions = new HashSet<Vector2Int>();
        foreach (GameObject door in doorList)
        {
            DestroyImmediate(door);
        }
        doorList.Clear();

        HashSet<Vector2Int> potentialRoomPositions = new HashSet<Vector2Int>();

        List<List<Vector2Int>> corridors = CreateCorridors(roadPositions, potentialRoomPositions);
        tileMapVisualizer.PaintRoadTiles(roadPositions);
        tileMapVisualizer.PaintRoadIntersectionTiles(roadPositions);

        for (int i = 0; i < corridors.Count; i++)
        {
            corridors[i] = IncreaseCorridorBrush(corridors[i], corridorSize);
            roadPositions.UnionWith(corridors[i]);
        }
        HashSet<Vector2Int> roomPositions = CreateHouses(potentialRoomPositions, roadPositions, doorPositions, doorList);

        floorPositions.UnionWith(roadPositions);
        floorPositions.UnionWith(roomPositions);

        tileMapVisualizer.PaintFloorTiles(roomPositions);
        var wallPositions = JapanWallGenerator.CreateWalls(roomPositions, roadPositions, tileMapVisualizer);
        var fencePositions = JapanWallGenerator.CreateFences(roadPositions, floorPositions, wallPositions, tileMapVisualizer);
        tileMapVisualizer.PaintGrassTiles(fencePositions);
        tileMapVisualizer.PaintRoofTiles(roomPositions, wallPositions, doorPositions);
        SpawnItems(roomPositions, roadPositions, doorPositions);
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
    private HashSet<Vector2Int> CreateHouses(HashSet<Vector2Int> potentialRoomPositions, HashSet<Vector2Int> roadPositions, HashSet<Vector2Int> doorPositions, List<GameObject> doorList)
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        int roomToCreateCount = Mathf.RoundToInt(potentialRoomPositions.Count);
        Debug.Log("==========Create "+roomToCreateCount+" Houses===========");

        List<Vector2Int> roomsToCreate = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomToCreateCount).ToList();
        var roomFloor = new HashSet<Vector2Int>();

        Vector2Int roomOffset2D = new Vector2Int(roomOffset-1, roomOffset);
        foreach (var roomPosition in roomsToCreate)
        {
            if (Math.Abs(roomPosition.x + roomOffset) <= initialAreaLength/2 && Math.Abs(roomPosition.y + roomOffset) <= initialAreaWidth/2) continue;

            roomFloor = CreateHouse(roomPosition + roomOffset2D, roadPositions, doorPositions, doorList);
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
        for (int i = 0; i < 4; i++)
        {
            List<Vector2Int> corridor = new List<Vector2Int>();
            for (int j = 1; j <= initialAreaLength+8; ++j)
            {
                currentPosition += Direction2D.cardinalDirectionsList[i];
                corridor.Add(currentPosition);
            }

            for (int x = 0; x < 4; x++)
            {
                List<Vector2Int> newcorridor = new List<Vector2Int>();
                Vector2Int edgePosition = currentPosition;
                for (int j = 1; j < maximumHouseLength + 10; j++)
                {
                    newcorridor.Add(currentPosition + Direction2D.cardinalDirectionsList[x] * j);
                }
                if (x == 3 || x == 4)
                {
                    edgePosition = newcorridor[newcorridor.Count - 1];
                    potentialRoomPositions.Add((Vector2Int)edgePosition);
                }
                floorPositions.UnionWith(newcorridor);
                corridors.Add(newcorridor);
            }

            potentialRoomPositions.Add((Vector2Int)currentPosition);
            corridors.Add(corridor);
            floorPositions.UnionWith(corridor);
        }

        for (int i = 0; i < corridorCount; i++) 
        {
            var corridor = ProceduralGeneration.RandomWalkCorridors(currentPosition, corridorLength);
            currentPosition = corridor[corridor.Count - 1];
            potentialRoomPositions.Add((Vector2Int)currentPosition);
            List<Vector2Int> newcorridor = new List<Vector2Int>();
            for (int x = 0; x < 2; x++)
            {
                for (int j = 1; j < maximumHouseLength + 10; j++)
                {
                    newcorridor.Add(currentPosition+Direction2D.cardinalDirectionsList[x]*j);
                }
            }
            corridors.Add(newcorridor);
            floorPositions.UnionWith(corridor);
            floorPositions.UnionWith(newcorridor);
            corridors.Add(corridor);
        }
        return corridors;
    }


    private HashSet<Vector2Int> CreateHouse(Vector2Int roomPosition, HashSet<Vector2Int> roadPositions, HashSet<Vector2Int> doorPositions, List<GameObject> doorList)
    {
        var roomsList = ProceduralGeneration.BinarySpacePartitioning(new BoundsInt((Vector3Int)roomPosition, new Vector3Int(maximumHouseLength, maximumHouseLength, 0)), minRoomWidth, minRoomHeight);
        int count = 0;
        while (roomsList.Count <= 1 && count<20)
        {
            roomsList = ProceduralGeneration.BinarySpacePartitioning(new BoundsInt((Vector3Int)roomPosition, new Vector3Int(maximumHouseLength, maximumHouseLength, 0)), minRoomWidth, minRoomHeight);
            count++;
        }
        Debug.Log("Room Retry Count: " + count);

        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        floor.Clear();
        floor = CreateSimpleRooms(roomsList, roadPositions, doorPositions, doorList);

        List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach (var room in roomsList)
        {
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }

        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);

        //Expand House Hallways to 2 wide
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

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList, HashSet<Vector2Int> roadPositions, HashSet<Vector2Int> doorPositions, List<GameObject> doorList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        var firstElement = roomsList.First();
        Vector3Int roomMinX = firstElement.min;
        Vector3Int roomMinY = firstElement.min;
        foreach(var room in roomsList)
        {
            if(room.min.x < roomMinX.x)
            {
                roomMinX = room.min;
            }

            if (room.min.y < roomMinY.y)
            {
                roomMinY = room.min;
            }
        }
        Vector2Int startPositionX = (Vector2Int)roomMinX + new Vector2Int(offset, offset);
        Vector2Int startPositionY = (Vector2Int)roomMinY + new Vector2Int(offset, offset);
        HashSet<Vector2Int> corridorX = new HashSet<Vector2Int>();
        HashSet<Vector2Int> corridorY = new HashSet<Vector2Int>();
        int countX = 0;
        int countY = 0;
        while (!roadPositions.Contains(startPositionX + Direction2D.cardinalDirectionsList[3]) && countX < 20)
        {
            corridorX.Add(startPositionX + Direction2D.cardinalDirectionsList[0]*2 + Direction2D.cardinalDirectionsList[3]);
            corridorX.Add(startPositionX + Direction2D.cardinalDirectionsList[0] + Direction2D.cardinalDirectionsList[3]);
            startPositionX = startPositionX + Direction2D.cardinalDirectionsList[3];
            countX++;

        }

        while (!roadPositions.Contains(startPositionY + Direction2D.cardinalDirectionsList[2]) && countY < 20)
        {
            corridorY.Add(startPositionY + Direction2D.cardinalDirectionsList[1]*2 + Direction2D.cardinalDirectionsList[2]);
            corridorY.Add(startPositionY + Direction2D.cardinalDirectionsList[1] + Direction2D.cardinalDirectionsList[2]);
            startPositionY = startPositionY + Direction2D.cardinalDirectionsList[2];
            countY++;
        }

        if (countX < countY)
        {
            floor.UnionWith(corridorX);

            GameObject door = Instantiate(doorVerticalPrefab, this.transform);
            doorList.Add(door);
            Vector2Int startPosition = (startPositionX + Direction2D.cardinalDirectionsList[0] * 2);
            Vector3 doorPosition = new Vector3(startPosition.x-0.5f, startPosition.y, 0);
            door.transform.localPosition = doorPosition;
            doorPositions.Add(startPositionX + Direction2D.cardinalDirectionsList[0] * 2);
            doorPositions.Add(startPositionX + Direction2D.cardinalDirectionsList[0]);
        }
        else
        {
            floor.UnionWith(corridorY);

            GameObject door = Instantiate(doorHorizontalPrefab, this.transform);
            doorList.Add(door);
            Vector2Int startPosition = (startPositionY + Direction2D.cardinalDirectionsList[1]);
            Vector3 doorPosition = new Vector3(startPosition.x + 0.5f, startPosition.y+0.5f, 0);
            door.transform.localPosition = doorPosition;
            doorPositions.Add(startPositionY + Direction2D.cardinalDirectionsList[1] * 2);
            doorPositions.Add(startPositionY + Direction2D.cardinalDirectionsList[1]);
        }

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
