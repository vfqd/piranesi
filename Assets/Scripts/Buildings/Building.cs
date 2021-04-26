using System.Collections.Generic;
using DG.Tweening;
using Framework;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Buildings
{
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
        public string requiredString;

        public TileMapState myTilemapState;
        public Vector3Int myCell;

        public int acolyteCount;
        public int acolytesMax;

        public List<GameObject> myAcolytes;

        public bool HasSpace => acolyteCount < acolytesMax;
    
        private SpriteRenderer _spriteRenderer;
        public bool placed;
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
                
                    if (!MapController.Instance.CheckTileString(tile, requiredString))
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
            acolyteCount = Mathf.Min(acolyteCount, acolytesMax);

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

        public void RemoveAcolyte()
        {
            if (_even)
            {
                if (acolyteCount > 0 && (acolyteCount - 1) % 2 == 0)
                {
                    myAcolytes[acolyteCount-1].SetActive(false);
                }
            }
            else
            {
                if (acolyteCount > 0 && (acolyteCount - 1) % 2 != 0)
                {
                    myAcolytes[acolyteCount-1].SetActive(false);
                }
            }
            
            acolyteCount--;
            acolyteCount = Mathf.Max(acolyteCount, 0);
        }

        public void Place(TileMapState state, Vector3Int cell)
        {
            ChangeColor(Color.white);
            myTilemapState = state;
            myCell = cell;
            MapController.Instance.buildings.Add(this);
            placed = true;
            ResourcesController.Instance.woodCount -= woodCost;
            ResourcesController.Instance.metalsCount -= metalCost;
        
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
}