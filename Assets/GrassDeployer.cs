using UnityEngine;
using UnityEngine.Tilemaps;

public class GrassDeployer : MonoBehaviour
{
    [SerializeField] Tilemap grassTilemap;
    [SerializeField] TileBase grassTile;
    [SerializeField] private int width, length = 0;

    public void PaintGrassTiles()
    {
        for (int i = -width / 2; i < width / 2; i++)
        {
            for (int j = -length / 2; j < length / 2; j++)
            {
                PaintSingleTile(grassTilemap, grassTile, new Vector2Int(j, i));
            }
        }
    }
    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }
}
