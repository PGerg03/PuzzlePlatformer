using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpecialTiles : MonoBehaviour
{
    #region Variables
    [SerializeField] Game GameScript;
    [SerializeField] Tilemap tilemap;
    public int JumpPadPower = 5;
    public List<CustomTile> tiles = new();
    public List<DefTileData> LoadadSpecialTiles = new();

    public static SpecialTiles instance;
    #endregion
    #region Functions
    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    void Start()
    {
        // Testing
        BoundsInt bounds = tilemap.cellBounds;

        for (int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                TileBase temp = tilemap.GetTile(new Vector3Int(x, y, 0));
                CustomTile temptile = tiles.Find(t => t.tile == temp);
                DefTileData tempdata = new();

                if (temptile != null)
                {
                    tempdata.tile = temptile.tileName;
                    tempdata.pos = new Vector3Int(x, y, 0);
                    LoadadSpecialTiles.Add(tempdata);
                }
            }
        }
        // Testing

    }

    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != 7) return;

        GameObject Player = collision.gameObject;
        Vector3 pos = Player.transform.localPosition;
        pos.y -= 1;
        pos.x -= 0.5f;

        float dist = 10;
        int index = 0;
        for (int i = 0; i < LoadadSpecialTiles.Count; i++)
        {
            float distance = (pos - LoadadSpecialTiles[i].pos).sqrMagnitude;

            if (dist > distance) index = i;
            dist = Math.Min(dist, distance);
        }
        string CollidedBlock = LoadadSpecialTiles[index].tile;

        if (GameScript.SinglePlayer)
        {
            switch (CollidedBlock)
            {
                case "TopDoor":
                case "BottomDoor":
                    GameScript.CompleteLevel();
                    break;
                case "JumpPad":
                    JumpPadJump(Player);
                    break;
                case "TopSpike":
                case "BottomSpike":
                case "LeftSpike":
                case "RightSpike":
                    SpikeHit();
                    break;
                default: return;
            }
        }
    }

    public void JumpPadJump(GameObject Player)
    {
        Rigidbody2D rg = Player.GetComponent<Rigidbody2D>();

        rg.velocity = new Vector2(rg.velocity.x, JumpPadPower);
    }

    public void SpikeHit()
    {
        GameScript.LoseLevel();
    }

    #endregion
    #region Save - Load
    public List<DefTileData> SaveSpecialTiles()
    {
        if (LoadadSpecialTiles.Count < 1) ReloadMap();

        return LoadadSpecialTiles;
    }

    public void ReloadMap()
    {
        BoundsInt bounds = tilemap.cellBounds;
        List<DefTileData> data = new();
        for (int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                TileBase temp = tilemap.GetTile(new Vector3Int(x, y, 0));
                CustomTile temptile = tiles.Find(t => t.tile == temp);
                DefTileData tempdata = new();

                if (temptile != null)
                {
                    tempdata.tile = temptile.tileName;
                    tempdata.pos = new Vector3Int(x, y, 0);
                    data.Add(tempdata);
                }
            }
        }

        LoadadSpecialTiles = data;
    }

    public void LoadSpecialTiles(List<DefTileData> data)
    {
        LoadadSpecialTiles = data;

        tilemap.ClearAllTiles();

        for (int i = 0; i < data.Count; i++)
        {
            tilemap.SetTile(data[i].pos, tiles.Find(t => t.tileName == data[i].tile).tile);
        }
    }
    #endregion
}
