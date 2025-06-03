using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using UnityEngine.WSA;

public class JapanTileMapVisualizer : MonoBehaviour
{
    [SerializeField] private Tilemap floorTilemap, roadTilemap, wallTilemap, wallFrontTilemap, fencesTilemap, treesTilemap, grassTilemap, roofTilemap;

    [SerializeField] private TileBase floorTile, roofTile,
                                      roadTile, roadVerti, roadVerti2, roadHori, roadHori2, roadMidLeft, roadMidRight, roadMidTop, roadMidBottom,
                                      roadCornerTopLeft, roadCornerTopRight, roadCornerBottomLeft, roadCornerBottomRight,
                                      roadInnerCornerTopLeft, roadInnerCornerTopRight, roadInnerCornerBottomLeft, roadInnerCornerBottomRight,
                                      roadIntersect, roadStripeVert, roadStripeHori, bushTile, bushTile2, bushTile3, bushTile4, treeTile,
                                      sideFull, sideMidLeft, sideMidRight, sideMidTop, sideMidBottom,
                                      sideCornerTopLeft, sideCornerTopRight, sideCornerBottomLeft, sideCornerBottomRight,
                                      sideInnerCornerTopLeft, sideInnerCornerTopRight, sideInnerCornerBottomLeft, sideInnerCornerBottomRight,
                                      sideRampUp, sideRampRight, sideRampDown, sideRampLeft,
                                      wallVerti, wallHori, wallFull,
                                      wallTUp, wallTDown, wallTLeft, wallTRight,
                                      wallCornerTopLeft, wallCornerTopRight, wallCornerBottomLeft, wallCornerBottomRight,
                                      wallEdgeUp, wallEdgeDown, wallEdgeLeft, wallEdgeRight,
                                      wallTopLeft, wallTopRight, wallBottomLeft, wallBottomRight, 
                                      wallIntersect, wallFront;

    [SerializeField] private List<TileBase> grassTiles;
    [SerializeField] private List<TileBase> fenceTiles;
    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        PaintTiles(floorPositions, floorTilemap, floorTile);
    }

    public void PaintRoofTiles(HashSet<Vector2Int> roofPositions, HashSet<Vector2Int> wallFrontPositions, HashSet<Vector2Int> doorPositions)
    {
        roofPositions.UnionWith(wallFrontPositions);
        foreach (var position in roofPositions)
        {
            if (!doorPositions.Contains(position))
            {
                PaintSingleTile(roofTilemap, roofTile, position);
            }
        }
    }

    public void PaintRoadIntersectionTiles(IEnumerable<Vector2Int> roadPositions)
    {
        foreach (Vector2Int position in roadPositions)
        {
            string binaryType = "";
            foreach (var direction in Direction2D.cardinalDirectionsList)
            {
                var neighborPosition = position + direction;
                if (roadPositions.Contains(neighborPosition))
                    binaryType += "1";
                else binaryType += "0";
            }

            int typeAsInt = Convert.ToInt32(binaryType, 2);

            if (WallTypesHelper.wallCornerTopLeft.Contains(typeAsInt)) //L-Bend DownRight
            {
                TileBase[] listOfTiles = {      sideFull,   sideMidLeft,            roadMidLeft,     roadVerti,          roadMidRight,                sideMidRight, sideFull,
                                           sideMidBottom, sideCornerBottomRight,    roadMidLeft,     roadVerti,          roadMidRight,                sideMidRight, sideFull,
                                           roadMidBottom, roadMidBottom, roadInnerCornerTopLeft,     roadVerti,          roadMidRight,                sideMidRight, sideFull,
                                                roadHori,      roadHori,               roadHori, roadIntersect,          roadMidRight,                sideMidRight, sideFull,
                                              roadMidTop,    roadMidTop,             roadMidTop,    roadMidTop, roadCornerBottomRight,                sideMidRight, sideFull,
                                              sideMidTop,    sideMidTop,             sideMidTop,    sideMidTop,            sideMidTop,  sideInnerCornerBottomRight, sideFull,
                                                sideFull,      sideFull,               sideFull,      sideFull,              sideFull,                    sideFull, sideFull};
                PaintSevenSquare(listOfTiles, position);
            }
            else if (WallTypesHelper.wallCornerTopRight.Contains(typeAsInt)) //L-Bend DownLeft
            {
                TileBase[] listOfTiles = {      sideFull,   sideMidLeft,          roadMidLeft,     roadVerti,            roadMidRight,         sideMidRight,      sideFull,
                                                sideFull,   sideMidLeft,          roadMidLeft,     roadVerti,            roadMidRight, sideCornerBottomLeft, sideMidBottom,
                                                sideFull,   sideMidLeft,          roadMidLeft,     roadVerti, roadInnerCornerTopRight,        roadMidBottom, roadMidBottom,
                                                sideFull,   sideMidLeft,          roadMidLeft, roadIntersect,                roadHori,             roadHori,      roadHori,
                                                sideFull,  sideMidLeft,  roadCornerBottomLeft,    roadMidTop,              roadMidTop,           roadMidTop,    roadMidTop,
                                                sideFull, sideInnerCornerBottomLeft, sideMidTop,  sideMidTop,              sideMidTop,           sideMidTop,    sideMidTop,
                                                sideFull,      sideFull,             sideFull,      sideFull,                sideFull,             sideFull,      sideFull};
                PaintSevenSquare(listOfTiles, position);
            }
            else if (WallTypesHelper.wallCornerBottomLeft.Contains(typeAsInt)) //L-Bend TopRight
            {
                TileBase[] listOfTiles = {      sideFull,      sideFull,                          sideFull,      sideFull,              sideFull,                    sideFull, sideFull,
                                              sideMidBottom,    sideMidBottom,               sideMidBottom, sideMidBottom,         sideMidBottom,   sideInnerCornerTopRight, sideFull,
                                              roadMidBottom,    roadMidBottom,               roadMidBottom, roadMidBottom, roadCornerTopRight,                sideMidRight, sideFull,
                                                roadHori,      roadHori,                          roadHori, roadIntersect,          roadMidRight,                sideMidRight, sideFull,
                                              roadMidTop,    roadMidTop,         roadInnerCornerBottomLeft,     roadVerti,          roadMidRight,                sideMidRight, sideFull,
                                              sideMidTop, sideCornerTopRight,               roadMidLeft,     roadVerti,          roadMidRight,                sideMidRight, sideFull,
                                                sideFull,   sideMidLeft,                       roadMidLeft,     roadVerti,          roadMidRight,                sideMidRight, sideFull};
                PaintSevenSquare(listOfTiles, position);
            }
            else if (WallTypesHelper.wallCornerBottomRight.Contains(typeAsInt)) //L-Bend TopLeft
            {
                TileBase[] listOfTiles = {      sideFull,      sideFull,               sideFull,      sideFull,                   sideFull,             sideFull,         sideFull,
                                                sideFull, sideInnerCornerTopLeft, sideMidBottom, sideMidBottom,              sideMidBottom,        sideMidBottom,    sideMidBottom,
                                                sideFull,   sideMidLeft,      roadCornerTopLeft, roadMidBottom,              roadMidBottom,        roadMidBottom,    roadMidBottom,
                                                sideFull,   sideMidLeft,            roadMidLeft, roadIntersect,                   roadHori,             roadHori,         roadHori,
                                                sideFull,  sideMidLeft,             roadMidLeft,     roadVerti, roadInnerCornerBottomRight,           roadMidTop,       roadMidTop,
                                                sideFull,   sideMidLeft,            roadMidLeft,     roadVerti,               roadMidRight,    sideCornerTopLeft,       sideMidTop,
                                                sideFull,   sideMidLeft,            roadMidLeft,     roadVerti,               roadMidRight,         sideMidRight,        sideFull};
                PaintSevenSquare(listOfTiles, position);
            }
            else if (WallTypesHelper.wallEdgeUp.Contains(typeAsInt)) //T-Intersection Up
            {
                TileBase[] listOfTiles = { sideFull,             sideRampRight,            roadStripeVert, roadStripeVert,             roadStripeVert,         sideRampLeft,       sideFull,
                                       sideRampDown,     sideCornerBottomRight,               roadMidLeft,       roadTile,               roadMidRight, sideCornerBottomLeft,   sideRampDown,
                                     roadStripeHori,             roadMidBottom,    roadInnerCornerTopLeft,       roadTile,    roadInnerCornerTopRight,        roadMidBottom, roadStripeHori,
                                     roadStripeHori,                  roadTile,                  roadTile,  roadIntersect,                   roadTile,             roadTile, roadStripeHori,
                                     roadStripeHori,                roadMidTop,                roadMidTop,     roadMidTop,                 roadMidTop,           roadMidTop, roadStripeHori,
                                         sideRampUp,                sideMidTop,                sideMidTop,     sideMidTop,                 sideMidTop,           sideMidTop,     sideRampUp,
                                           sideFull,                  sideFull,                  sideFull,       sideFull,                   sideFull,             sideFull,       sideFull};
                PaintSevenSquare(listOfTiles, position);
            }
            else if (WallTypesHelper.wallEdgeDown.Contains(typeAsInt)) //T-InterSection Down
            {
                TileBase[] listOfTiles = { sideFull,                  sideFull,                  sideFull,       sideFull,                   sideFull,             sideFull,       sideFull,
                                       sideRampDown,             sideMidBottom,             sideMidBottom,  sideMidBottom,              sideMidBottom,        sideMidBottom,    sideRampDown,
                                     roadStripeHori,             roadMidBottom,             roadMidBottom,  roadMidBottom,              roadMidBottom,        roadMidBottom, roadStripeHori,
                                     roadStripeHori,                  roadTile,                  roadTile,  roadIntersect,                   roadTile,             roadTile, roadStripeHori,
                                     roadStripeHori,                roadMidTop, roadInnerCornerBottomLeft,       roadTile, roadInnerCornerBottomRight,           roadMidTop, roadStripeHori,
                                         sideRampUp,        sideCornerTopRight,               roadMidLeft,       roadTile,               roadMidRight,    sideCornerTopLeft,     sideRampUp,
                                           sideFull,             sideRampRight,            roadStripeVert, roadStripeVert,             roadStripeVert,         sideMidRight,       sideFull};
                PaintSevenSquare(listOfTiles, position);
            }
            else if (WallTypesHelper.wallEdgeLeft.Contains(typeAsInt)) //T-Intersection Left
            {
                TileBase[] listOfTiles = { sideFull,             sideRampRight,            roadStripeVert, roadStripeVert,             roadStripeVert,         sideRampLeft, sideFull,
                                       sideRampDown,     sideCornerBottomRight,               roadMidLeft,       roadTile,               roadMidRight,         sideMidRight, sideFull,
                                     roadStripeHori,             roadMidBottom,    roadInnerCornerTopLeft,       roadTile,               roadMidRight,         sideMidRight, sideFull,
                                     roadStripeHori,                  roadTile,                  roadTile,  roadIntersect,               roadMidRight,         sideMidRight, sideFull,
                                     roadStripeHori,                roadMidTop, roadInnerCornerBottomLeft,       roadTile,               roadMidRight,         sideMidRight, sideFull,
                                         sideRampUp,        sideCornerTopRight,               roadMidLeft,       roadTile,               roadMidRight,         sideMidRight, sideFull,
                                           sideFull,             sideRampRight,            roadStripeVert, roadStripeVert,             roadStripeVert,         sideRampLeft, sideFull};
                PaintSevenSquare(listOfTiles, position);
            }
            else if (WallTypesHelper.wallEdgeRight.Contains(typeAsInt)) //T-Intersection Right
            {
                TileBase[] listOfTiles = { sideFull,             sideRampRight,            roadStripeVert, roadStripeVert,             roadStripeVert,         sideRampLeft,       sideFull,
                                           sideFull,               sideMidLeft,               roadMidLeft,       roadTile,               roadMidRight, sideCornerBottomLeft,   sideRampDown,
                                           sideFull,               sideMidLeft,               roadMidLeft,       roadTile,    roadInnerCornerTopRight,        roadMidBottom, roadStripeHori,
                                           sideFull,               sideMidLeft,               roadMidLeft,  roadIntersect,                   roadTile,             roadTile, roadStripeHori,
                                           sideFull,               sideMidLeft,               roadMidLeft,       roadTile, roadInnerCornerBottomRight,           roadMidTop, roadStripeHori,
                                           sideFull,               sideMidLeft,               roadMidLeft,       roadTile,               roadMidRight,    sideCornerTopLeft,     sideRampUp,
                                           sideFull,             sideRampRight,            roadStripeVert, roadStripeVert,             roadStripeVert,         sideMidRight,       sideFull};
                PaintSevenSquare(listOfTiles, position);
            }
            else if (WallTypesHelper.wallFull.Contains(typeAsInt)) //Full InterSection
            {
                TileBase[] listOfTiles = { sideFull,             sideRampRight,            roadStripeVert, roadStripeVert,             roadStripeVert,         sideRampLeft,       sideFull,
                                       sideRampDown,     sideCornerBottomRight,               roadMidLeft,       roadTile,               roadMidRight, sideCornerBottomLeft,   sideRampDown,
                                     roadStripeHori,             roadMidBottom,    roadInnerCornerTopLeft,       roadTile,    roadInnerCornerTopRight,        roadMidBottom, roadStripeHori,
                                     roadStripeHori,                  roadTile,                  roadTile,  roadIntersect,                   roadTile,             roadTile, roadStripeHori,
                                     roadStripeHori,                roadMidTop, roadInnerCornerBottomLeft,       roadTile, roadInnerCornerBottomRight,           roadMidTop, roadStripeHori,
                                         sideRampUp,        sideCornerTopRight,               roadMidLeft,       roadTile,               roadMidRight,    sideCornerTopLeft,     sideRampUp,
                                           sideFull,             sideRampRight,            roadStripeVert, roadStripeVert,             roadStripeVert,        sideRampLeft,       sideFull};
                PaintSevenSquare(listOfTiles, position);
            }
        }
    }
    internal void PaintSevenSquare(TileBase[] listOfTiles, Vector2Int position)
    {
        int count = 0;
        for (int y = 3; y >= -3; y--)
        {
            for (int x = -3; x < 4; x++)
            {
                if (listOfTiles[count] != null) PaintSingleTile(roadTilemap, listOfTiles[count], position + new Vector2Int(x, y));
                count++;
            }
        }
    }
    public void PaintRoadTiles(IEnumerable<Vector2Int> roadPositions)
    {
        foreach (Vector2Int position in roadPositions)
        {
            string binaryType = "";
            foreach (var direction in Direction2D.cardinalDirectionsList)
            {
                var neighborPosition = position + direction;
                if (roadPositions.Contains(neighborPosition))
                    binaryType += "1";
                else binaryType += "0";
            }

            int typeAsInt = Convert.ToInt32(binaryType, 2);

            if (WallTypesHelper.wallHori.Contains(typeAsInt))
            {
                for(int i = -3; i < 4; i++){
                    PaintSingleTile(roadTilemap, sideFull, position + Direction2D.cardinalDirectionsList[3] * 3 + Direction2D.cardinalDirectionsList[0]*i);
                    PaintSingleTile(roadTilemap, sideMidLeft, position + Direction2D.cardinalDirectionsList[3] * 2 + Direction2D.cardinalDirectionsList[0] * i);
                    PaintSingleTile(roadTilemap, roadMidLeft, position + Direction2D.cardinalDirectionsList[3] + Direction2D.cardinalDirectionsList[0] * i);
                    PaintSingleTile(roadTilemap, roadVerti, position + Direction2D.cardinalDirectionsList[0] * i);
                    PaintSingleTile(roadTilemap, roadMidRight, position + Direction2D.cardinalDirectionsList[1] + Direction2D.cardinalDirectionsList[0] * i);
                    PaintSingleTile(roadTilemap, sideMidRight, position + Direction2D.cardinalDirectionsList[1] * 2 + Direction2D.cardinalDirectionsList[0] * i);
                    PaintSingleTile(roadTilemap, sideFull, position + Direction2D.cardinalDirectionsList[1] * 3 + Direction2D.cardinalDirectionsList[0] * i);
                }

            }
            else if (WallTypesHelper.wallVerti.Contains(typeAsInt))
            {
                for (int i = -3; i < 4; i++)
                {
                    PaintSingleTile(roadTilemap, roadMidBottom, position + Direction2D.cardinalDirectionsList[0] + Direction2D.cardinalDirectionsList[1] * i);
                    PaintSingleTile(roadTilemap, sideMidBottom, position + Direction2D.cardinalDirectionsList[0] * 2 + Direction2D.cardinalDirectionsList[1] * i);
                    PaintSingleTile(roadTilemap, sideFull, position + Direction2D.cardinalDirectionsList[0] * 3 + Direction2D.cardinalDirectionsList[1] * i);
                    PaintSingleTile(roadTilemap, roadHori, position + Direction2D.cardinalDirectionsList[1] * i);
                    PaintSingleTile(roadTilemap, sideFull, position + Direction2D.cardinalDirectionsList[2] * 3 + Direction2D.cardinalDirectionsList[1] * i);
                    PaintSingleTile(roadTilemap, sideMidTop, position + Direction2D.cardinalDirectionsList[2] * 2 + Direction2D.cardinalDirectionsList[1] * i);
                    PaintSingleTile(roadTilemap, roadMidTop, position + Direction2D.cardinalDirectionsList[2] + Direction2D.cardinalDirectionsList[1] * i);
                }
            }
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
        fencesTilemap.ClearAllTiles();
        roadTilemap.ClearAllTiles();
        grassTilemap.ClearAllTiles();
        roofTilemap.ClearAllTiles();
        treesTilemap.ClearAllTiles();
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

    public void PaintGrassTiles(HashSet<Vector2Int> fencePositions)
    {
        int minX = 0, minY = 0;
        int maxX = 0, maxY = 0;

        foreach (var position in fencePositions)
        {
            minX = Math.Min(minX, position.x);
            minY = Math.Min(minY, position.y);
            maxX = Math.Max(maxX, position.x);
            maxY = Math.Max(maxY, position.y);
        }
        for (int x = minX - 10; x < maxX + 10; x++)
        {
            for (int y = minY - 10; y < maxY + 10; y++)
            {
                int randNum = UnityEngine.Random.Range(0, grassTiles.Count());
                PaintSingleTile(grassTilemap, grassTiles[randNum], new Vector2Int(x, y));
            }
        }
    }
    internal void PaintSingleBasicFence(Vector2Int position)
    {
        TileBase fenceTile = bushTile;
        switch (UnityEngine.Random.Range(0,4))
        {
            case 0:
                fenceTile = bushTile;
                break;

            case 1: 
                fenceTile = bushTile2;
                break;

            case 2:
                fenceTile = bushTile3;
                break;

            case 3:
                fenceTile = bushTile4;
                break;

        }
        PaintSingleTile(fencesTilemap, fenceTile, position);
    }

    internal void PaintSingleBasicTree(Vector2Int position)
    {
        PaintSingleTile(treesTilemap, treeTile, position);
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
        foreach (var pos in roadTilemap.cellBounds.allPositionsWithin)
        {
            if (roadTilemap.HasTile(pos))
            {
                Vector3 worldPos = roadTilemap.CellToWorld(pos);
                positions.Add(new Vector2Int(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.y)));


            }
        }
        return positions;
    }
}
