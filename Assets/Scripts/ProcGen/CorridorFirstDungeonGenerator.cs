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
    [SerializeField] private GameObject doorHorizontalPrefab;
    [SerializeField] private GameObject doorVerticalPrefab;
    [SerializeField] private int minimumRoomLength = 6, maximumRoomLength = 17;
    [SerializeField] private int initialRoomLength, initialRoomWidth = 10;
    [SerializeField] private int corridorSize = 3;
    [SerializeField] private bool randomWalk = false;
    [SerializeField] private List<GameObject> doorList;

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
        HashSet<Vector2Int> corridorPositions = new HashSet<Vector2Int>();

        foreach (GameObject door in doorList)
        {
            DestroyImmediate(door);
        }
        doorList.Clear();

        List<List<Vector2Int>> corridors = CreateCorridors(floorPositions, potentialRoomPositions);

        HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomPositions);

        List<Vector2Int> deadEnds = FindAllDeadEnds(floorPositions);

        CreateRoomsAtDeadEnd(deadEnds, roomPositions);

        floorPositions.UnionWith(roomPositions);

        for (int i = 0; i < corridors.Count; i++)
        {
            corridors[i] = IncreaseCorridorBrush(corridors[i],corridorSize);
            floorPositions.UnionWith(corridors[i]);
            corridorPositions.UnionWith(corridors[i]);
        }

        HashSet<Vector2Int> doorPositions = FindDoorPositions(floorPositions, roomPositions);
        CreateDoors(doorPositions);
        tileMapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions,tileMapVisualizer);
        SpawnItems(roomPositions,doorPositions);
    }

    private void CreateDoors(HashSet<Vector2Int> doorPositions)
    {
        if (!doorHorizontalPrefab || !doorVerticalPrefab) return;

        foreach (var position in doorPositions)
        {
            if(doorPositions.Contains(position + Direction2D.cardinalDirectionsList[1]))
            {
                GameObject door = Instantiate(doorHorizontalPrefab, this.transform);
                doorList.Add(door);
                Vector3 doorPosition = new Vector3(position.x + 0.5f, position.y, 0);
                door.transform.localPosition = doorPosition;
            }
            else if (doorPositions.Contains(position + Direction2D.cardinalDirectionsList[2]))
            {
                GameObject door = Instantiate(doorVerticalPrefab, this.transform);
                doorList.Add(door);
                Vector3 doorPosition = new Vector3(position.x - 0.5f, position.y, 0);
                door.transform.localPosition = doorPosition;
            }
        }
    }

    private HashSet<Vector2Int> FindDoorPositions(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> roomPositions)
    {
        HashSet<Vector2Int> doorPositions = new HashSet<Vector2Int>();
        foreach (Vector2Int pos in roomPositions)
        {
            for(int i = 0; i < 4; i++) 
            {
                if (floorPositions.Contains(pos + Direction2D.cardinalDirectionsList[i])&&!roomPositions.Contains(pos + Direction2D.cardinalDirectionsList[i]))
                {
                    doorPositions.Add(pos + Direction2D.cardinalDirectionsList[i]);
                    for(int j = 0; j < 3; j++)
                    {
                        if (floorPositions.Contains(pos + Direction2D.cardinalDirectionsList[i]+Direction2D.cardinalDirectionsList[(i+1)%4]) && !roomPositions.Contains(pos + Direction2D.cardinalDirectionsList[i]+Direction2D.cardinalDirectionsList[(i + 1)%4]))
                        {
                            doorPositions.Add(pos + Direction2D.cardinalDirectionsList[i]+Direction2D.cardinalDirectionsList[(i+1)%4]);
                        }
                        else if (floorPositions.Contains(pos + Direction2D.cardinalDirectionsList[i]+Direction2D.cardinalDirectionsList[(i + 3) % 4]) && !roomPositions.Contains(pos + Direction2D.cardinalDirectionsList[i]+Direction2D.cardinalDirectionsList[(i + 3) % 4]))
                        {
                            doorPositions.Add(pos + Direction2D.cardinalDirectionsList[i] + Direction2D.cardinalDirectionsList[(i + 3) % 4]);
                        }
                    }
                    
                }
            }
        }
        return doorPositions;
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
        roomFloor = RunRectangleWalk(new Vector2Int(0, 0), initialRoomWidth, initialRoomLength);
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
        
        foreach (Vector2Int direction in Direction2D.cardinalDirectionsList)
        {
            currentPosition = startPosition;
            List<Vector2Int> corridor = new List<Vector2Int>();
            for (int i = 0; i < corridorLength + 1; i++)
            {
                corridor.Add(startPosition + direction * i);
            }
            currentPosition = corridor[corridor.Count - 1];
            potentialRoomPositions.Add((Vector2Int)currentPosition);
            floorPositions.UnionWith(corridor);
            corridors.Add(corridor);
            for (int i = 0; i < corridorCount; i++)
            {
                corridor = ProceduralGeneration.RandomWalkCorridors(currentPosition, corridorLength);
                currentPosition = corridor[corridor.Count - 1];
                potentialRoomPositions.Add((Vector2Int)currentPosition);
                floorPositions.UnionWith(corridor);
                corridors.Add(corridor);
            }
        }



        return corridors;
    }
}
