using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapVisualizer : MonoBehaviour
{
    [SerializeField] private Tilemap floorTilemap, wallTilemap, itemFloorTilemap;

    [SerializeField] private TileBase floorTile, wallTop, wallFront;

    [SerializeField] public List<Item_ScriptableObj> items = new List<Item_ScriptableObj>();    
    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        if (UnityEngine.Random.value < 0.5f)
        {
            
        }
        else
        {
            PaintTiles(floorPositions, floorTilemap, floorTile);
        }
    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
    {
        foreach (Vector2Int position in positions)
        {
            PaintSingleTile(tilemap, tile, position);
        }
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    public void Clear()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        itemFloorTilemap.ClearAllTiles();
    }

    internal void PaintSingleBasicWallTop(Vector2Int position)
    {
        PaintSingleTile(wallTilemap, wallTop, position);
    }
    internal void PaintSingleBasicWallFront(Vector2Int position)
    {
        PaintSingleTile(wallTilemap, wallFront, position);
    }
}
