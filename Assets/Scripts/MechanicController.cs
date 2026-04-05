using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

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
        if (instance == null) instance = this;
        else Destroy(this);
    }

    void Start()
    {
        //Testing
        BoundsInt bounds = tilemap.cellBounds;

        for (int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                TileBase temp = tilemap.GetTile(new Vector3Int(x, y, 0));
                CustomTile temptile = tiles.Find(t => t.tile == temp);
                ButtonData tempdata = new();

                if (temptile != null)
                {
                    tempdata.button = temptile.tileName;
                    tempdata.pos = new Vector3Int(x, y, 0);
                    tempdata.gatesIndex.Add(Buttons.Count);
                    Buttons.Add(tempdata);
                }
            }
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            GateData tempdata = new();

            Transform child = transform.GetChild(i);
            tempdata.gate = child.name;
            tempdata.pos = child.localPosition;
            tempdata.rot = (int)child.transform.eulerAngles.z;
            Gates.Add(tempdata);
            GateObjects.Add(child.gameObject);
        }
        //Testing
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
        for (int i = 0; i < Buttons.Count; i++)
        {
            float distance = (pos - Buttons[i].pos).sqrMagnitude;

            if (dist > distance) index = i;
            dist = Math.Min(dist, distance);
        }

        string CollidedBlock = Buttons[index].button;
        ButtonData activatedButton = Buttons[index];

        Player.GetComponent<PlayerMovement>().LastContact = index;

        int color;
        switch (CollidedBlock)
        {
            case "BlueButton":
            case "BluePressurePlate":
                color = 1;
                break;
            case "GreenButton":
            case "GreenPressurePlate":
                color = 2;
                break;
            case "RedButton":
            case "RedPressurePlate":
                color = 3;
                break;
            case "YellowButton":
            case "YellowPressurePlate":
                color = 4;
                break;
            default: return;
        }
        if (color > 0) ActivateButton(activatedButton, color);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer != 0) return;

        GameObject Player = collision.gameObject;

        int index = Player.GetComponent<PlayerMovement>().LastContact;
        if (index < 0) return;

        string CollidedBlock = Buttons[index].button;
        ButtonData activatedButton = Buttons[index];

        int color;
        switch (CollidedBlock)
        {
            case "BluePressurePlate":
                color = 1;
                break;
            case "GreenPressurePlate":
                color = 2;
                break;
            case "RedPressurePlate":
                color = 3;
                break;
            case "YellowPressurePlate":
                color = 4;
                break;
            default: return;
        }
        if (color > 0) DeactivateButton(activatedButton, color);
    }

    public void ActivateButton(ButtonData activeButton, int color)
    {
        ChangeButtonStatus(activeButton.pos, color, true);

        for (int i = 0; i < activeButton.gatesIndex.Count; i++)
        {
            int index = activeButton.gatesIndex[i];
            GateObjects[index].GetComponent<Animator>().SetBool("Open", true);
            GateObjects[index].GetComponent<Animator>().SetBool("Close", false);
        }
    }

    public void DeactivateButton(ButtonData deactiveButton, int color)
    {
        ChangeButtonStatus(deactiveButton.pos, color, false);

        for (int i = 0; i < deactiveButton.gatesIndex.Count; i++)
        {
            int index = deactiveButton.gatesIndex[i];
            GateObjects[index].GetComponent<Animator>().SetBool("Close", true);
            GateObjects[index].GetComponent<Animator>().SetBool("Open", false);
        }
    }

    /// <param name="active">true ha benyomva van, false ha felengedve</param>
    public void ChangeButtonStatus(Vector3Int pos, int color, bool active)
    {
        if (active) tilemap.SetTile(pos, tiles[7 + color].tile);
        else tilemap.SetTile(pos, tiles[3 + color].tile);
    }

    #endregion
    #region Save & Load
    public List<ButtonData> SaveButtons()
    {
        if (Buttons.Count < 1) ReloadMap();

        return Buttons;
    }

    public List<GateData> SaveGates()
    {
        if (Gates.Count < 1) ReloadMap();

        return Gates;
    }

    public void ReloadMap()
    {
        BoundsInt bounds = tilemap.cellBounds;
        List<ButtonData> data = new();

        for (int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                TileBase temp = tilemap.GetTile(new Vector3Int(x, y, 0));
                CustomTile temptile = tiles.Find(t => t.tile == temp);
                ButtonData tempdata = new();

                if (temptile != null)
                {
                    tempdata.button = temptile.tileName;
                    tempdata.pos = new Vector3Int(x, y, 0);
                    tempdata.gatesIndex.Add(-1);
                    data.Add(tempdata);
                }
            }
        }

        Buttons = data;

        List<GateData> gdata = new();
        for (int i = 0; i < transform.childCount; i++)
        {
            GateData tempgdata = new();

            Transform child = transform.GetChild(i);
            tempgdata.gate = child.name;
            tempgdata.pos = child.localPosition;
            tempgdata.rot = (int)child.transform.eulerAngles.z;
            gdata.Add(tempgdata);
        }

        Gates = gdata;
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
