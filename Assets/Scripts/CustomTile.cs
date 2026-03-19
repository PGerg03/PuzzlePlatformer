using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New CustomTile")]
public class CustomTile : ScriptableObject
{
    public TileBase tile;
    public string tileName;
}