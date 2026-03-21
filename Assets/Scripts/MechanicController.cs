using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MechanicController : MonoBehaviour
{
    #region Variables
    [SerializeField] Game GameScript;
    [SerializeField] Tilemap tilemap;
    [SerializeField] List<CustomTile> tiles = new();
    [SerializeField] List<GameObject> gateModels = new();
    public List<GateData> Gates = new();
    public List<ButtonData> Buttons = new();
    public List<GameObject> GateObjects = new();

    public static MechanicController instance;
    #endregion
    #region Functions
    void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(this);
    }
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer != 0) return;

        GameObject Player = collision.gameObject;

        string CollidedBlock = "";
        List<int> activatedGates = new();

        for (int i = 0; i < Buttons.Count; i++)
        {
            float distance = (Player.transform.localPosition - Buttons[i].pos).sqrMagnitude;

            if(distance < 4f)
            {
                CollidedBlock = Buttons[i].button;
                activatedGates.AddRange(Buttons[i].gatesIndex);
                Debug.Log(CollidedBlock);
                break;
            }
        }

        // if(GameScript.SinglePlayer)
        {
            switch(CollidedBlock)
            {
                case "RedButton" :
                case "BlueButton" :
                case "GreenButton" :
                case "YellowButton" :
                    ActivateButton(activatedGates, false);
                    break;
                case "RedPressurePlate" :
                case "BluePressurePlate" :
                case "GreenPressurePlate" :
                case "YellowPressurePlate" :
                    ActivateButton(activatedGates, true);
                    break;
                default: return;
            }
        }
    }

    public void ActivateButton(List<int> activeGate, bool Plate)
    {
        if(Plate)
        {
            
        }
        else
        {
            
        }
    }

    #endregion
    #region Save & Load
    public List<ButtonData> SaveButtons()
    {
        BoundsInt bounds = tilemap.cellBounds;
        List<ButtonData> data = new();

        for(int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for(int y = bounds.min.y; y < bounds.max.y; y++)
            {
                TileBase temp = tilemap.GetTile(new Vector3Int(x, y, 0));
                CustomTile temptile = tiles.Find(t => t.tile == temp);
                ButtonData tempdata = new();

                if(temptile != null)
                {
                    tempdata.button = temptile.tileName;
                    tempdata.pos = new Vector3Int(x, y, 0);
                    tempdata.gatesIndex.Add(-1);
                    data.Add(tempdata);
                }
            }
        }

        return data;
    }

    public List<GateData> SaveGates()
    {
        List<GateData> data = new();
        for (int i = 0; i < transform.childCount; i++)
        {
            GateData tempdata = new();

            Transform child = transform.GetChild(i);
            tempdata.gate = child.name;
            tempdata.pos = child.localPosition;
            tempdata.rot = (int)child.transform.rotation.z;
            data.Add(tempdata);
        }

        return data;
    }

    public void LoadMechanicTiles(List<ButtonData> bData, List<GateData> gData)
    {
        Buttons = bData;
        Gates = gData;

        tilemap.ClearAllTiles();
        for (int i = 0; i < GateObjects.Count; i++)
        {
            Destroy(GateObjects[i]);
        }
        GateObjects.Clear();


        for (int i = 0; i < bData.Count; i++)
        {
            tilemap.SetTile(bData[i].pos, tiles.Find(t => t.tileName == bData[i].button).tile);
        }
        for (int i = 0; i < gData.Count; i++)
        {
            GameObject newGate = gateModels.Find(g => gData[i].gate.StartsWith(g.name));
            newGate.transform.position = gData[i].pos;
            newGate.transform.eulerAngles = new Vector3Int(0, 0, gData[i].rot);

            GateObjects.Add(Instantiate(newGate, transform));
        }
    }


    #endregion
}
