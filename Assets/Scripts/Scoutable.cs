using System;
using Buildings;
using SoundManager;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Scoutable : MonoBehaviour
{
    public float progress;
    public float timeNeeded;
    public bool isActive;

    public TextMeshPro text;
    public SpriteRenderer spriteRenderer;
    public int hallToUnlock;

    public EffectSoundBank effectSoundBank;

    private void OnMouseDown()
    {
        foreach (var scoutable in GameObject.FindObjectsOfType<Scoutable>())
        {
            if (scoutable != this)
            {
                scoutable.isActive = false;
            }
        }
        print("PressDown");

        isActive = true;
    }

    private void Update()
    {
        int towerCount = 1;
        foreach (var building in MapController.Instance.buildings)
        {
            if (building.type == Building.Type.Watchpost) towerCount++;
        }

        float percent = progress / timeNeeded;
        
        if (isActive)
        {
            spriteRenderer.color = Color.grey;
            text.text = "Scouting... " + Mathf.RoundToInt(percent * 100) + "%";
            progress += towerCount * Time.deltaTime;
        }
        else
        {
            spriteRenderer.color = Color.white;
            
            if (percent > 0)
                text.text = "Resume Scouting";
            else 
                text.text = "Start Scouting";
        }
        
        if (percent >= 1)
        {
            if (!MapController.Instance.tilemaps[hallToUnlock].explored)
            {
                MapController.Instance.tilemaps[hallToUnlock].Explore();
                effectSoundBank.Play();
                Destroy(gameObject);
            }
        }
        
        
    }
}