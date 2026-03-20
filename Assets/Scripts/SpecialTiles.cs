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
    public SpecialTilesData LoadadSpecialTiles = new();

    public static SpecialTiles instance;
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

                if(temptile != null)
                {
                    LoadadSpecialTiles.tiles.Add(temptile.tileName);
                    LoadadSpecialTiles.pos.Add(new Vector3Int(x, y, 0));
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
        if(collision.gameObject.layer != 0) return;

        GameObject Player = collision.gameObject;

        string CollidedBlock = "";

        for (int i = 0; i < LoadadSpecialTiles.tiles.Count; i++)
        {
            float distance = (Player.transform.localPosition - LoadadSpecialTiles.pos[i]).sqrMagnitude;

            if(distance < 4f)
            {
                CollidedBlock = LoadadSpecialTiles.tiles[i];
                Debug.Log(CollidedBlock);
                break;
            }
        }

        if(GameScript.SinglePlayer)
        {
            switch(CollidedBlock)
            {
                case "TopDoor":
                case "BottomDoor":
                    GameScript.CompleteLevel();
                    break;
                case "JumpPad":
                    JumpPadJump(Player);
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

    #endregion
    #region Save - Load
    public SpecialTilesData SaveSpecialTiles()
    {
        BoundsInt bounds = tilemap.cellBounds;
        SpecialTilesData data = new();
        for(int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for(int y = bounds.min.y; y < bounds.max.y; y++)
            {
                TileBase temp = tilemap.GetTile(new Vector3Int(x, y, 0));
                CustomTile temptile = tiles.Find(t => t.tile == temp);

                if(temptile != null)
                {
                    data.tiles.Add(temptile.tileName);
                    data.pos.Add(new Vector3Int(x, y, 0));
                }
            }
        }

        return data;
    }
    public void LoadSpecialTiles(SpecialTilesData data)
    {
        LoadadSpecialTiles = data;

        tilemap.ClearAllTiles();

        for (int i = 0; i < data.tiles.Count; i++)
        {
            tilemap.SetTile(data.pos[i], tiles.Find(t => t.tileName == data.tiles[i]).tile);
        }
    }
    #endregion
}
