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
    public List<GameObject> Players;

    public List<CustomTile> tiles = new();
    
    public static TileMapController instance;
    void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(this);
    }

    void FixedUpdate()
    {
        if(EditMode) GameScript.inGame = false;
        Players.ForEach(p => p.GetComponent<Rigidbody2D>().simulated = GameScript.inGame);
    }

    #region Edit
    void Update()
    {
        if(EditMode && Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F)) SaveMap();
        if(EditMode && Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R)) ReloadMaps();
        
    }
    public bool EditMode = true;
    public string SaveName = "SingleLevel1";
    private void ReloadMaps()
    {
        SpecialTiles.instance.ReloadMap();
        MechanicController.instance.ReloadMap();
        WaterController.instance.ReloadMap();
        BackgroundController.instance.ReloadMap();
    }

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
                DefTileData maintile = new();

                if(temptile != null)
                {
                    maintile.pos = new Vector3Int(x,y,0);
                    maintile.tile = temptile.tileName;
                    levelData.MaintileData.Add(maintile);
                }
            }
        }

        Players.ForEach(p => levelData.PlayersPos.Add(new Vector3Int((int)p.transform.position.x, (int)p.transform.position.y, 0)));

        levelData.specialTiles = SpecialTiles.instance.SaveSpecialTiles();
        levelData.buttonsData = MechanicController.instance.SaveButtons();
        levelData.gatesData = MechanicController.instance.SaveGates();
        levelData.waterData = WaterController.instance.SaveWaterTiles();
        levelData.backgroundData = BackgroundController.instance.SaveBackTiles();

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

        for (int i = 0; i < data.MaintileData.Count; i++)
        {
            tilemap.SetTile(new Vector3Int(data.MaintileData[i].pos.x, data.MaintileData[i].pos.y, 0), tiles.Find(t => t.tileName == data.MaintileData[i].tile).tile);
        }

        SpecialTiles.instance.LoadSpecialTiles(data.specialTiles);
        MechanicController.instance.LoadMechanicTiles(data.buttonsData, data.gatesData);
        WaterController.instance.LoadWaterTiles(data.waterData);
        BackgroundController.instance.LoadBackTiles(data.backgroundData);
        
        for (int i = 0; i < data.PlayersPos.Count; i++)
        {
            if(Players.Count == i)
            {
                Players.Add(GameScript.CreateNextPlayer(i));
            }
            Players[i].transform.localPosition = data.PlayersPos[i];
            Players[i].GetComponent<PlayerMovement>().LastContact = -1;
        }

        return data.Story;
    }

    #endregion
}

#region DataClasses
[System.Serializable]
public class LevelData
{
    public string Story;
    public List<Vector3Int> PlayersPos = new();
    public List<DefTileData> MaintileData = new();
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

#endregion