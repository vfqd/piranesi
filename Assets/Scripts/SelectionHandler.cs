using System;
using Framework;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;
using Random = UnityEngine.Random;

public class SelectionHandler : MonoSingleton<SelectionHandler>
{
    public GameObject selectionIndicator;
    
    private void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        selectionIndicator.gameObject.SetActive(false);
        foreach (var state in MapController.Instance.tilemaps)
        {
            if (!state.explored) continue;
            
            var v3 = state.tilemap.WorldToCell(mousePos);
            if (state.tilemap.HasTile(v3))
            {
                TileBase tile = state.tilemap.GetTile(v3);
                if (tile.name.ToLower().Contains("tile"))
                {
                    print(tile);
                    selectionIndicator.gameObject.SetActive(true);
                    selectionIndicator.transform.position = state.tilemap.CellToWorld(v3).PlusY(.25f);
                }
            }
        }
    }
}