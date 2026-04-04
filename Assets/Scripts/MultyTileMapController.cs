using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MultyTileMapController : MonoBehaviour
{
    public Tilemap MainTilemap;
    public Tilemap SpecialTilemap;

    public TileBase specialTile; 

    void Start()
    {
        BoundsInt bounds = MainTilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);

                TileBase tileA = MainTilemap.GetTile(pos);
                TileBase tileB = SpecialTilemap.GetTile(pos);

                if (tileA != null && tileB != null)
                {
                    MainTilemap.SetTile(pos, specialTile);
                }
            }
        }
    }

    public void UpdateTilesBasedOnOtherTilemap()
    {
        BoundsInt bounds = MainTilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);

                TileBase tileA = MainTilemap.GetTile(pos);
                TileBase tileB = SpecialTilemap.GetTile(pos);

                if (tileA != null && tileB != null)
                {
                    MainTilemap.SetTile(pos, specialTile);
                }
                else if (tileA != null)
                {
                    // Ha tileB nincs, visszaállíthatod az eredeti tile-t vagy más logikát alkalmazhatsz
                    // tilemapA.SetTile(pos, originalTile);
                }
            }
        }
    }
}
