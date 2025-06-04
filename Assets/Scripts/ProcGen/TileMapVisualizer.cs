using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class TileMapVisualizer : MonoBehaviour
{
    [SerializeField] private Tilemap floorTilemap, wallTilemap, wallFrontTilemap;

    [SerializeField] private TileBase floorTile,floorTile2, wallVerti, wallHori, wallFull,
                                      wallTUp, wallTDown, wallTLeft, wallTRight,
                                      wallCornerTopLeft, wallCornerTopRight, wallCornerBottomLeft, wallCornerBottomRight,
                                      wallEdgeUp, wallEdgeDown, wallEdgeLeft, wallEdgeRight,
                                      wallTopLeft, wallTopRight, wallBottomLeft, wallBottomRight, 
                                      wallIntersect, wallFront;
    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        PaintTiles(floorPositions, floorTilemap, floorTile);
    }

    public void PaintColoredTiles(HashSet<Vector2Int> roomPositions, HashSet<Vector2Int> corridorPositions)
    {
        foreach (Vector2Int position in roomPositions)
        {
            PaintSingleTile(floorTilemap, floorTile, position);
        }
        foreach (Vector2Int position in corridorPositions)
        {
            PaintSingleTile(floorTilemap, floorTile2, position);
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
        wallFrontTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
    }

    internal void PaintSingleBasicWall(Vector2Int position, string binaryType, string binaryWallType)
    {
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        int walltypeAsInt = Convert.ToInt32(binaryWallType, 2);
        TileBase tile = null;
        if (WallTypesHelper.wallTUp.Contains(typeAsInt) && WallTypesHelper.wallEdgeDown.Contains(walltypeAsInt))
        {
            tile = wallTUp;
        }
        else if (WallTypesHelper.wallTDown.Contains(typeAsInt) && WallTypesHelper.wallEdgeUp.Contains(walltypeAsInt))
        {
            tile = wallTDown;
        }
        else if (WallTypesHelper.wallTLeft.Contains(typeAsInt) && WallTypesHelper.wallEdgeRight.Contains(walltypeAsInt))
        {
            tile = wallTLeft;
        }
        else if (WallTypesHelper.wallTRight.Contains(typeAsInt) && WallTypesHelper.wallEdgeLeft.Contains(walltypeAsInt))
        {
            tile = wallTRight;
        }
        else if (WallTypesHelper.wallVerti.Contains(typeAsInt))
        {
            tile = wallVerti;
        }
        else if (WallTypesHelper.wallHori.Contains(typeAsInt))
        {
            tile = wallHori;
        }
        else if (WallTypesHelper.wallFull.Contains(typeAsInt))
        {
            tile = wallFull;
        }
        else if (WallTypesHelper.wallCornerTopLeft.Contains(typeAsInt))
        {
            tile = wallCornerTopLeft;
        }
        else if (WallTypesHelper.wallCornerTopRight.Contains(typeAsInt))
        {
            tile = wallCornerTopRight;
        }
        else if (WallTypesHelper.wallCornerBottomLeft.Contains(typeAsInt))
        {
            tile = wallCornerBottomLeft;
        }
        else if (WallTypesHelper.wallCornerBottomRight.Contains(typeAsInt))
        {
            tile = wallCornerBottomRight;
        }
        else if (WallTypesHelper.wallEdgeUp.Contains(typeAsInt))
        {
            tile = wallEdgeUp;
        }
        else if (WallTypesHelper.wallEdgeDown.Contains(typeAsInt))
        {
            tile = wallEdgeDown;
        }
        else if (WallTypesHelper.wallEdgeLeft.Contains(typeAsInt))
        {
            tile = wallEdgeLeft;
        }
        else if (WallTypesHelper.wallEdgeRight.Contains(typeAsInt))
        {
            tile = wallEdgeRight;
        }

        if (tile != null)
        {
            var down = Direction2D.cardinalDirectionsList[2];
            PaintSingleTile(wallTilemap, tile, position);
            PaintSingleTile(wallFrontTilemap, wallFront, position+down);
        }
    }

    internal void PaintSingleCornerWall(Vector2Int position, string binaryType, string binaryWallType)
    {
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        int walltypeAsInt = Convert.ToInt32(binaryWallType, 2);
        TileBase tile = null;

        if (WallTypesHelper.wallEdgeDown.Contains(walltypeAsInt))
        {
            tile = wallTUp;
        }
        else if (WallTypesHelper.wallEdgeUp.Contains(walltypeAsInt))
        {
            tile = wallTDown;
        }
        else if (WallTypesHelper.wallEdgeRight.Contains(walltypeAsInt))
        {
            tile = wallTLeft;
        }
        else if (WallTypesHelper.wallEdgeLeft.Contains(walltypeAsInt))
        {
            tile = wallTRight;
        }
        else if (WallTypesHelper.wallInnerCornerDownLeft.Contains(typeAsInt))
        {
            tile = wallTopRight;
        }
        else if (WallTypesHelper.wallInnerCornerDownRight.Contains(typeAsInt))
        {
            tile = wallTopLeft;
        }
        else if (WallTypesHelper.wallDiagonalCornerDownLeft.Contains(typeAsInt))
        {
            tile = wallBottomLeft;
        }
        else if (WallTypesHelper.wallDiagonalCornerDownRight.Contains(typeAsInt))
        {
            tile = wallBottomRight;
        }
        else if (WallTypesHelper.wallDiagonalCornerUpLeft.Contains(typeAsInt))
        {
            tile = wallTopLeft;
        }
        else if (WallTypesHelper.wallDiagonalCornerUpRight.Contains(typeAsInt))
        {
            tile = wallTopRight;
        }
        else if (WallTypesHelper.wallFull.Contains(walltypeAsInt))
        {
            tile = wallIntersect;
        }
        else if (WallTypesHelper.wallFullEightDirections.Contains(typeAsInt))
        {
            tile = wallFull;
        }
        else if (WallTypesHelper.wallBottomEightDirections.Contains(typeAsInt))
        {
            tile = wallFull;
        }

        if (tile != null)
        {
            var down = Direction2D.cardinalDirectionsList[2];
            PaintSingleTile(wallTilemap, tile, position);
            PaintSingleTile(wallFrontTilemap, wallFront, position + down);
        }

    }
   public List<Vector2Int> GetFloorWorldPositions()
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        foreach (var pos in floorTilemap.cellBounds.allPositionsWithin)
        {
            if (floorTilemap.HasTile(pos))
            {
                Vector3 worldPos = floorTilemap.CellToWorld(pos);
                positions.Add(new Vector2Int(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.y)));


            }
        }

        return positions;
    }


}
