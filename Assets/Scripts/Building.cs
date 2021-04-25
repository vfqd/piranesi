using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Framework;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class Building : MonoBehaviour
{
    public enum Type
    {
        Compound,
        Pen,
        Watchpost,
        Altar,
        Fishery,
        Lumbercamp,
        Mine
    }

    public Type type;
    public int woodCost;
    public int metalCost;

    [HideInInspector] public string description;
    
    public int size;
    public bool canFlipSprite;
    public string requiredAtLeastOneOfString;

    public TileMapState myTilemapState;
    public Vector3Int myCell;

    public int acolyteCount;
    public int acolytesMax;

    public List<GameObject> myAcolytes;

    public bool HasSpace => acolyteCount < acolytesMax;
    
    private SpriteRenderer _spriteRenderer;
    private bool _placed;
    private bool _even;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        myAcolytes.Shuffle();
        if (canFlipSprite)
        {
            _spriteRenderer.flipX = Random.value < 0.5f;
        }

        _even = Random.value < 0.5f;
    }

    public virtual bool IsBuildingAllowedHere(TileMapState state, Vector3Int cell)
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                var v3 = new Vector3Int(cell.x + i, cell.y + j, 0);
                TileBase tile = state.tilemap.GetTile(v3);

                if (!tile || state.buildingCoords.ContainsKey(v3))
                {
                    return false;
                }
                
                if (!MapController.Instance.CheckTileString(tile, "tile"))
                {
                    return false;
                }

                // if (!requiredAtLeastOneOfString.IsNullOrEmpty() &&
                //     MapController.Instance.CheckTileString(tile, requiredAtLeastOneOfString))
                // {
                //     
                // }
            }
        }
        return true;
    }

    public void AssignAcolyte()
    {
        acolyteCount++;

        if (_even)
        {
            if ((acolyteCount - 1) % 2 == 0)
            {
                myAcolytes[acolyteCount-1].SetActive(true);
            }
        }
        else
        {
            if ((acolyteCount - 1) % 2 != 0)
            {
                myAcolytes[acolyteCount-1].SetActive(true);
            }
        }
    }

    public void Place(TileMapState state, Vector3Int cell)
    {
        ChangeColor(Color.white);
        myTilemapState = state;
        myCell = cell;
        MapController.Instance.buildings.Add(this);
        _placed = true;
        
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                state.buildingCoords.Add(new Vector3Int(cell.x + i, cell.y + j, 0), this);
            }
        }
        
        CameraController.Instance.SmallCameraShake();
        transform.DOPunchScale(Vector3.one * .1f, .33f, 1, .1f);
    }

    public void ChangeColor(Color c)
    {
        _spriteRenderer.color = c;
    }
}