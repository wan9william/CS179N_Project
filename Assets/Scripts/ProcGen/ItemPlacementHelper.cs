using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class ItemPlacementHelper
{
    //Dictionary<PlacementType, HashSet<Vector2Int>>
    //    tileByType = new Dictionary<PlacementType, HashSet<Vector2Int>>();

    HashSet<Vector2Int> roomFloorNoCorridor;
    
    public ItemPlacementHelper(HashSet<Vector2Int> roomFloor,
        HashSet<Vector2Int> roomFloorNoCorridor)
    {
        Graph graph = new Graph(roomFloor);
        this.roomFloorNoCorridor = roomFloorNoCorridor;
        foreach (var position in roomFloorNoCorridor)
        {
            int neighborsCount8Dir = graph.GetNeighbors8Directions(position).Count;
            //PlacementType type = neighborsCount8Dir < 8 ? PlacementType.NearWall : PlacementType.Open;

        }
    }
}
