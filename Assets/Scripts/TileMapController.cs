using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapController : MonoBehaviour
{
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
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(cam.transform.position.z - tilemap.transform.position.z);
        Vector3Int pos = tilemap.WorldToCell(cam.ScreenToWorldPoint(mousePos));

        if(EditMode && Input.GetMouseButton(0))
        {
            PlaceTile(pos);
        }

        if(EditMode && Input.GetMouseButton(1))
        {
            DeleteTile(pos);
        }

        if(EditMode && Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.L))
        {
            SaveMap();
        }
    }
    public bool EditMode = true;
    public string SaveName = "SingleLevel1";

    void FixedUpdate()
    {
        Player1.GetComponent<Rigidbody2D>().simulated = !EditMode;
    }
    private void PlaceTile(Vector3Int pos)
    {
        tilemap.SetTile(pos, CurrentTile);
    }
    private void DeleteTile(Vector3Int pos)
    {
        tilemap.SetTile(pos, null);
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

        string json = JsonUtility.ToJson(levelData, true);
        File.WriteAllText(Application.dataPath + $"/Maps/{SaveName}.json", json);
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
        
        Player1.transform.localPosition = data.Player1Pos;
        if(!Player2.IsUnityNull()) 
            Player2.transform.localPosition = data.Player2Pos;

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
    public SpecialTilesData specialTiles = new();
    public ButtonsData buttonsData = new();
    public GatesData gatesData = new();
}

[System.Serializable]
public class SpecialTilesData
{
    public List<string> tiles = new();
    public List<Vector3Int> pos = new();
}

[System.Serializable]
public class ButtonsData
{
    public List<string> buttons = new();
    public List<Vector3Int> pos = new();
    public List<List<int>> gatesIndex = new();
}

[System.Serializable]
public class GatesData
{
    public List<string> gates = new();
    public List<Transform> trans = new();
}
