using System;
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

    
    private void Start()
    {
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

    }
}