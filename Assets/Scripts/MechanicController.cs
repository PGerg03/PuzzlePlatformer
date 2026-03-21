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
    public GatesData Gates = new();
    public ButtonsData Buttons = new();
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

        for (int i = 0; i < Buttons.buttons.Count; i++)
        {
            float distance = (Player.transform.localPosition - Buttons.pos[i]).sqrMagnitude;

            if(distance < 4f)
            {
                CollidedBlock = Buttons.buttons[i];
                activatedGates.AddRange(Buttons.gatesIndex[i]);
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

                    break;
                case "RedPressurePlate" :
                case "BluePressurePlate" :
                case "GreenPressurePlate" :
                case "YellowPressurePlate" :

                    break;
                default: return;
            }
        }
    }


    public void LoadMechanicTiles(ButtonsData bData, GatesData gData)
    {
        Buttons = bData;
        Gates = gData;

        tilemap.ClearAllTiles();

        for (int i = 0; i < bData.buttons.Count; i++)
        {
            tilemap.SetTile(bData.pos[i], tiles.Find(t => t.tileName == bData.buttons[i]).tile);
        }
        for (int i = 0; i < gData.gates.Count; i++)
        {
            GameObject newGate = gateModels.Find(g => g.name == gData.gates[i]);
            newGate.transform.position = gData.trans[i].position;
            newGate.transform.eulerAngles = gData.trans[i].eulerAngles;

            GateObjects.Add(Instantiate(newGate));
        }
    }
    #endregion
}
