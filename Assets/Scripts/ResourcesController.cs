﻿using System;
using TMPro;
using UnityEngine;
using Utils;

public class ResourcesController : MonoSingleton<ResourcesController>
{
    public TextMeshProUGUI acolytesText, prisonersText, hierophantText, foodText, woodText, metalsText;
    
    public int acolytesCount, prisonersCount, hierophantCount, foodCount, woodCount, metalsCount;

    public int acolytesMax, prisonersMax;

    private void Update()
    {
        acolytesMax = 0;
        prisonersMax = 0;
        foreach (var building in MapController.Instance.buildings)
        {
            if (building.type == Building.Type.Compound) acolytesMax += 5;
            if (building.type == Building.Type.Pen) prisonersMax += 3;
        }

        acolytesText.text = acolytesCount + "/" + acolytesMax;
        prisonersText.text = prisonersCount + "/" + prisonersMax;
        hierophantText.text = hierophantCount.ToString();
        foodText.text = foodCount.ToString();
        woodText.text = woodCount.ToString();
        metalsText.text = metalsCount.ToString();
    }
}