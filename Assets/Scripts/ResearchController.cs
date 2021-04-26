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

    public int storedResearch = 0;
    
    public Image currentIcon;

    public Image[] progressBars;
    
    private void Start()
    {
    }

    private float _storedResearchDt = 0;

    private void Update()
    {
        if (currentResearch)
        {
            _storedResearchDt += Time.deltaTime;
            float percent = (float) currentResearch.progress / currentResearch.cost;
            currentPercent.text = currentResearch.progress + "/" + currentResearch.cost;

            if (storedResearch > 0 && _storedResearchDt > 1)
            {
                _storedResearchDt = 0;
                IncreaseProgressOfCurrentResearch(1);
                storedResearch--;
            }
            
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
            currentDesc.text = "";
            currentIcon.enabled = false;
        }
    }

    public void IncreaseProgressOfCurrentResearch(int amt)
    {
        if (!currentResearch) return;
        
        if (currentResearch.IncreaseProgress(amt))
        {
            currentResearch = null;
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