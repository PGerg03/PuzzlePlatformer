using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BackgroundController : MonoBehaviour
{
    
    #region Variables
    [SerializeField] Game GameScript;
    [SerializeField] Tilemap tilemap;
    public List<CustomTile> tiles = new();
    public List<DefTileData> LoadadBackgroundTiles = new();

    public static BackgroundController instance;
    #endregion
    #region Functions
    void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(this);
    }

    void Start()
    {
        // Testing
        BoundsInt bounds = tilemap.cellBounds;

        for(int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for(int y = bounds.min.y; y < bounds.max.y; y++)
            {
                TileBase temp = tilemap.GetTile(new Vector3Int(x, y, 0));
                CustomTile temptile = tiles.Find(t => t.tile == temp);
                DefTileData tempdata = new();

                if(temptile != null)
                {
                    tempdata.tile = temptile.tileName;
                    tempdata.pos = new Vector3Int(x, y, 0);
                    LoadadBackgroundTiles.Add(tempdata);
                }
            }
        }
        // Testing
        
    }

    void Update()
    {
        
    }

    #endregion
    #region Save - Load
    public List<DefTileData> SaveBackTiles()
    {
        if(LoadadBackgroundTiles.Count < 1) ReloadMap();
        
        return LoadadBackgroundTiles;
    }

    public void ReloadMap()
    {
        BoundsInt bounds = tilemap.cellBounds;
        List<DefTileData> data = new();
        for(int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for(int y = bounds.min.y; y < bounds.max.y; y++)
            {
                TileBase temp = tilemap.GetTile(new Vector3Int(x, y, 0));
                CustomTile temptile = tiles.Find(t => t.tile == temp);
                DefTileData tempdata = new();

                if(temptile != null)
                {
                    tempdata.tile = temptile.tileName;
                    tempdata.pos = new Vector3Int(x, y, 0);
                    data.Add(tempdata);
                }
            }
        }

        LoadadBackgroundTiles = data;
    }
    public void LoadBackTiles(List<DefTileData> data)
    {
        LoadadBackgroundTiles = data;

        tilemap.ClearAllTiles();

        for (int i = 0; i < data.Count; i++)
        {
            tilemap.SetTile(data[i].pos, tiles.Find(t => t.tileName == data[i].tile).tile);
        }
    }
    
    #endregion
}
