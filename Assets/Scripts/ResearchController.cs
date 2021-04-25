using System;
using System.Collections.Generic;
using Researches;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils;

public class ResearchController : MonoSingleton<ResearchController>
{
    public GameObject panel;
    public Research currentResearch;

    public TextMeshProUGUI currentName;
    public TextMeshProUGUI currentNameBar;
    public TextMeshProUGUI currentDesc;
    public TextMeshProUGUI currentPercent;
    
    public Image currentIcon;

    public Image[] progressBars;
    
    private void Start()
    {
    }

    private void Update()
    {
        if (currentResearch)
        {
            float percent = (float) currentResearch.progress / currentResearch.cost;
            currentPercent.text = Mathf.RoundToInt(percent*100) + "%";

            foreach (var progress in progressBars)
            {
                progress.fillAmount = percent;
            }
            
            currentName.text = currentNameBar.text = currentResearch.name;
        }
        else
        {
            currentPercent.text = "";
            foreach (var progress in progressBars)
            {
                progress.fillAmount = 0;
            }
            currentName.text = currentNameBar.text = "";
        }
    }

    public void SelectResearch(Research research)
    {
        currentResearch = research;
        currentName.text = currentNameBar.text = research.name;
        currentDesc.text = research.description;
        currentIcon.enabled = true;
        currentIcon.sprite = research.iconRenderer.sprite;
    }

    public void ToggleResearchPanel()
    {
        print("kakakakkaka");

        if (!panel.activeSelf)
        {
            panel.SetActive(true);
        }
        else
        {
            panel.SetActive(false);
        }
    }
}