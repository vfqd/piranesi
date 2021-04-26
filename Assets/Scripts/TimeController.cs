using System;
using Buildings;
using Febucci.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class TimeController : MonoSingleton<TimeController>
{
    public Light sun;
    public Gradient ambient;
    public float lengthOfDayInSeconds;
    public float currentTime;
    public TextMeshProUGUI speedText;
    
    [Range(0, 1)] public float becomeSunriseAt;
    [Range(0, 1)] public float becomeDayAt;
    [Range(0, 1)] public float becomeSunsetAt;
    [Range(0, 1)] public float becomeNightAt;

    private bool _sunrise, _day, _sunset, _night;
    
    public Image icon;
    public TextAnimatorPlayer timeOfDay;

    public Sprite sunSprite, moonSprite, sunDownSprite;

    private void Update()
    {
        if (!GameController.Instance.hasLeftLondon)
        {
            timeOfDay.GetComponent<TextMeshProUGUI>().text = "Night";
            return;
        }
        
        currentTime += Time.deltaTime;
        if (currentTime >= lengthOfDayInSeconds)
        {
            currentTime = 0;
            _sunrise = _day = _sunset = _night = false;
        }
        sun.color = ambient.Evaluate(currentTime / lengthOfDayInSeconds);

        if (currentTime/lengthOfDayInSeconds > becomeSunriseAt && !_sunrise)
        {
            _sunrise = true;
            icon.sprite = sunDownSprite;
            timeOfDay.ShowText("Sunrise");
            Portal.Instance.ProduceResources();
            
            foreach (var building in MapController.Instance.buildings)
            {
                if (building.type == Building.Type.Compound)
                {
                    building.GetComponent<Compound>().TakeCosts();
                }
            }
            ResearchController.Instance.IncreaseProgressOfCurrentResearch(ResourcesController.Instance.hierophantCount*2);
        }
        
        if (currentTime/lengthOfDayInSeconds > becomeDayAt && !_day)
        {
            _day = true;
            icon.sprite = sunSprite;
            timeOfDay.ShowText("Day");
        }
        
        if (currentTime/lengthOfDayInSeconds > becomeSunsetAt && !_sunset)
        {
            _sunset = true;
            icon.sprite = sunDownSprite;
            timeOfDay.ShowText("Sunset");

            if (GameController.Instance.createHierophants)
            {
                foreach (var building in MapController.Instance.buildings)
                {
                    if (building.type == Building.Type.Altar)
                    {
                        building.GetComponent<Altar>().CreateHierophant();
                    }
                }
            }
        }
        
        if (currentTime/lengthOfDayInSeconds > becomeNightAt && !_night)
        {
            _night = true;
            icon.sprite = moonSprite;
            timeOfDay.ShowText("Night");
        }
    }

    public void ToggleDoubleSpeed()
    {
        if (Time.timeScale > 1.1f)
        {
            Time.timeScale = 1;
            speedText.text = "x1";
        }
        else
        {
            Time.timeScale = 3;
            speedText.text = "x3";
        }
    }

    public float GetLengthOfDayForCalculations()
    {
        return GameController.Instance.hasLeftLondon ? lengthOfDayInSeconds : 10;
    }
}