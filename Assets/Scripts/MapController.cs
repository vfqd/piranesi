using System;
using System.Collections.Generic;
using Framework;
using OneLine;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;
using Random = UnityEngine.Random;

public class MapController : MonoSingleton<MapController>
{
    [OneLine] public TileMapState[] tilemaps;
    
    public Animator steamCloud;

    public List<Building> buildings;

    public int joblessAcolytes;
    
    private void Start()
    {
        buildings = new List<Building>();
        for (int k = 0; k < tilemaps.Length; k++)
        {
            TileMapState state = tilemaps[k];

            if (state.explored)
            {
                continue;
            }
            else
            {
                for (int i = -64; i < 64; i++)
                {
                    for (int j = -64; j < 64; j++)
                    {
                        Vector3Int cell = new Vector3Int(i, j, 0);

                        if (state.tilemap.HasTile(cell))
                        {
                            Animator anim = Instantiate(steamCloud, state.tilemap.CellToWorld(cell).PlusY(-0.5f), Quaternion.identity, transform);
                            AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
                            anim.GetComponent<SpriteRenderer>().flipX = Random.value < 0.25f;
                            anim.Play(info.fullPathHash, -1, Random.Range(0f, 1f));
                        }
                    }
                }
            }
        }
    }

    private void Update()
    {
        if (joblessAcolytes > 0)
        {
            foreach (var building in buildings)
            {
                if ((building.type == Building.Type.Altar || 
                     building.type == Building.Type.Watchpost || 
                     building.type == Building.Type.Fishery ||
                     building.type == Building.Type.Lumbercamp ||
                     building.type == Building.Type.Mine) &&
                    building.HasSpace)
                {
                    building.AssignAcolyte();
                    print("Assign to job");
                    joblessAcolytes--;
                }
                if (joblessAcolytes <= 0) break;
            }
        }
    }
    

    public void AddNewAcolytes(int count)
    {
        int toFindHomesFor = count;
        
        for (int i = 0; i < count; i++)
        {
            if (toFindHomesFor <= 0) break;
            foreach (var building in buildings)
            {
                if (building.type == Building.Type.Compound && building.HasSpace)
                {
                    building.AssignAcolyte();
                    ResourcesController.Instance.acolytesCount++;
                    toFindHomesFor--;
                    joblessAcolytes++;
                }
                if (toFindHomesFor <= 0) break;
            }
        }
    }
    

    public bool CheckTileString(TileBase tile, string keyword)
    {
        if (!tile) return false;
        return tile.name.ToLower().Contains(keyword.ToLower());
    }
}