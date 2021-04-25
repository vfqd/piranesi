using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class TileMapState
{
    public bool explored;
    public Tilemap tilemap;

    public Dictionary<Vector3Int, Building> buildingCoords = new Dictionary<Vector3Int, Building>();
}