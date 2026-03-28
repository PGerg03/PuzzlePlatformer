using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapController : MonoBehaviour
{
    [SerializeField] Game GameScript;
    [SerializeField] TileBase CurrentTile;
    [SerializeField] Camera cam;
    [SerializeField] Tilemap tilemap;
    [SerializeField] GameObject Player1;
    [SerializeField] GameObject Player2;

    public List<CustomTile> tiles = new();
    
    public static TileMapController instance;
    void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(this);
    }

    #region Edit
    void Update()
    {
        // Vector3 mousePos = Input.mousePosition;
        // mousePos.z = Mathf.Abs(cam.transform.position.z - tilemap.transform.position.z);
        // Vector3Int pos = tilemap.WorldToCell(cam.ScreenToWorldPoint(mousePos));
        // if(EditMode && Input.GetMouseButton(0))
        // {
        //     PlaceTile(pos);
        // }
        // if(EditMode && Input.GetMouseButton(1))
        // {
        //     DeleteTile(pos);
        // }

        if(EditMode && Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.L))
        {
            SaveMap();
        }
    }
    public bool EditMode = true;
    public string SaveName = "SingleLevel1";

    void FixedUpdate()
    {
        if(EditMode) GameScript.inGame = false;
        Player1.GetComponent<Rigidbody2D>().simulated = GameScript.inGame;
    }
    // private void PlaceTile(Vector3Int pos)
    // {
    //     tilemap.SetTile(pos, CurrentTile);
    // }
    // private void DeleteTile(Vector3Int pos)
    // {
    //     tilemap.SetTile(pos, null);
    // }
    
    private void SaveMap()
    {
        BoundsInt bounds = tilemap.cellBounds;

        LevelData levelData = new();

        for(int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for(int y = bounds.min.y; y < bounds.max.y; y++)
            {
                TileBase temp = tilemap.GetTile(new Vector3Int(x, y, 0));
                CustomTile temptile = tiles.Find(t => t.tile == temp);


                if(temptile != null)
                {
                    levelData.tiles.Add(temptile.tileName);
                    levelData.posx.Add(x);
                    levelData.posy.Add(y);
                }
            }
        }
        levelData.Player1Pos = new Vector3Int((int)Player1.transform.position.x, (int)Player1.transform.position.y, 0);

        levelData.specialTiles = SpecialTiles.instance.SaveSpecialTiles();
        levelData.buttonsData = MechanicController.instance.SaveButtons();
        levelData.gatesData = MechanicController.instance.SaveGates();

        string json = JsonUtility.ToJson(levelData, true);
        File.WriteAllText(Application.dataPath + $"/Maps/{SaveName}.json", json);

        Debug.Log($"Save Complete: /Maps/{SaveName}.json");
    }

    #endregion
    #region Game
    public string LoadMap(int map, string type)
    {
        SaveName = $"{type}Level{map}";
        if(!File.Exists(Application.dataPath + $"/Maps/{type}Level{map}.json")) return ""; // for Edit mode

        string json = File.ReadAllText(Application.dataPath + $"/Maps/{type}Level{map}.json");
        LevelData data = JsonUtility.FromJson<LevelData>(json);

        tilemap.ClearAllTiles();

        for (int i = 0; i < data.tiles.Count; i++)
        {
            tilemap.SetTile(new Vector3Int(data.posx[i], data.posy[i], 0), tiles.Find(t => t.tileName == data.tiles[i]).tile);
        }

        SpecialTiles.instance.LoadSpecialTiles(data.specialTiles);
        MechanicController.instance.LoadMechanicTiles(data.buttonsData, data.gatesData);
        WaterController.instance.LoadWaterTiles(data.waterData);
        BackgroundController.instance.LoadBackTiles(data.backgroundData);
        
        Player1.transform.localPosition = data.Player1Pos;
        Player1.GetComponent<PlayerMovement>().LastContact = -1;
        if(!Player2.IsUnityNull()) 
        {
            Player2.transform.localPosition = data.Player2Pos;
            Player2.GetComponent<PlayerMovement>().LastContact = -1;
        }
        return data.Story;
    }

    #endregion
}

[System.Serializable]
public class LevelData
{
    public string Story;
    public Vector3Int Player1Pos;
    public Vector3Int Player2Pos;
    public List<string> tiles = new();
    public List<int> posx = new();
    public List<int> posy = new();
    public List<DefTileData> backgroundData = new();
    public List<DefTileData> specialTiles = new();
    public List<ButtonData> buttonsData = new();
    public List<GateData> gatesData = new();
    public List<DefTileData> waterData = new();
}

[System.Serializable]
public class DefTileData
{
    public string tile = "";
    public Vector3Int pos = new();
}

[System.Serializable]
public class ButtonData
{
    public string button = "";
    public Vector3Int pos = new();
    public List<int> gatesIndex = new();
}

[System.Serializable]
public class GateData
{
    public string gate = "";
    public Vector3 pos = new();
    public int rot = 0;
}
