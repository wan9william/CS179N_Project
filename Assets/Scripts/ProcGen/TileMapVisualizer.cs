using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapVisualizer : MonoBehaviour
{
    [SerializeField] private Tilemap floorTilemap, wallTilemap;

    [SerializeField] private TileBase floorTile, wallTop, wallFront;

    [SerializeField] private ItemManager itemManager;
    [SerializeField] private bool spawnItems = false;
    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        PaintTiles(floorPositions, floorTilemap, floorTile);
    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
    {
        foreach (Vector2Int position in positions)
        {
            if(spawnItems) itemManager.InstantiateLoot(new Vector3(position.x,position.y,0), this.transform);
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
        itemManager.Clear();
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
