using System;
using Buildings;
using Framework;
using SoundManager;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Utils;
using Random = UnityEngine.Random;

public class SelectionHandler : MonoSingleton<SelectionHandler>
{
    public GameObject selectionIndicator;

    public Button compoundButton, altarButton, penButton, fisheryButton, lumbercampButton, mineButton, towerButton;

    private Building _currentlySelectedBuilding;
    private Vector3Int _currentlyHoveredOver;

    public EffectSoundBank placeBuilding, click;
    
    private void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            click.Play();
        }
        
        selectionIndicator.gameObject.SetActive(false);
        foreach (var state in MapController.Instance.tilemaps)
        {
            if (!state.explored) continue;

            var v3 = state.tilemap.WorldToCell(mousePos);
            if (state.tilemap.HasTile(v3))
            {
                _currentlyHoveredOver = v3;
                TileBase tile = state.tilemap.GetTile(v3);
                if (MapController.Instance.CheckTileString(tile,"tile") ||
                    MapController.Instance.CheckTileString(tile,"water") ||
                    MapController.Instance.CheckTileString(tile,"tree") ||
                    MapController.Instance.CheckTileString(tile,"mine"))
                {
                    if (_currentlySelectedBuilding)
                    {
                        var worldPos = state.tilemap.CellToWorld(v3);
                        var placementPos = new Vector3(worldPos.x, worldPos.y + .25f, worldPos.z + worldPos.y / 100f);
                        _currentlySelectedBuilding.transform.position = placementPos;
                        bool allowed = _currentlySelectedBuilding.IsBuildingAllowedHere(state, v3);
                        if (allowed)
                        {
                            _currentlySelectedBuilding.ChangeColor(Color.green);
                        }
                        else
                        {
                            _currentlySelectedBuilding.ChangeColor(Color.red);
                        }

                        if (allowed && Input.GetMouseButtonDown(0))
                        {
                            _currentlySelectedBuilding.Place(state, v3);
                            placeBuilding.Play();
                            _currentlySelectedBuilding = null;
                        }
                        else if (Input.GetMouseButtonDown(1))
                        {
                            DeselectBuilding();
                        }
                    }
                    else
                    {
                        selectionIndicator.gameObject.SetActive(true);
                        selectionIndicator.transform.position = state.tilemap.CellToWorld(v3).PlusY(.25f);
                    }
                }
            }
            // else
            // {
            //     if (_currentlySelectedBuilding)
            //     {
            //         _currentlySelectedBuilding.transform.position = mousePos;
            //     }
            // }
        }
    }

    public void PressBuildingButton(Building building)
    {
        DeselectBuilding();
        _currentlySelectedBuilding = Instantiate(building, new Vector2(-100,-100), Quaternion.identity);
    }

    private void DeselectBuilding()
    {
        if (_currentlySelectedBuilding)
        {
            Destroy(_currentlySelectedBuilding.gameObject);
            _currentlySelectedBuilding = null;
        }
    }
}