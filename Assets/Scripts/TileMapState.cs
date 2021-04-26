using System;
using System.Collections.Generic;
using Buildings;
using DG.Tweening;
using Researches;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class TileMapState
{
    public bool explored;
    public Tilemap tilemap;
    public List<Research> toUnlockWhenExplored;
    [TextArea]
    public string messageWhenExplored;

    [HideInInspector]
    public List<GameObject> fogClouds = new List<GameObject>();

    public Dictionary<Vector3Int, Building> buildingCoords = new Dictionary<Vector3Int, Building>();

    public void Explore()
    {
        if (explored) return;
        
        explored = true;
        Debug.Log("Explore " + fogClouds.Count);
        DialogController.Instance.PlayDialog(messageWhenExplored);

        bool newResearches = false;
        foreach (var research in toUnlockWhenExplored)
        {
            research.isVisible = true;
            newResearches = true;
        }
        if (newResearches) DialogController.Instance.PlayDialog("New rites have been uncovered");

        foreach (var fogCloud in fogClouds)
        {
            Debug.Log("ClearFog");
            fogCloud.GetComponent<SpriteRenderer>().DOColor(Color.clear, 5)
                .SetEase(Ease.Linear)
                .OnComplete(() => GameObject.Destroy(fogCloud));
        }
    }
}