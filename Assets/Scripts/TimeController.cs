using System;
using Febucci.UI;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class TimeController : MonoSingleton<TimeController>
{
    public Light sun;
    public Gradient ambient;
    public float lengthOfDayInSeconds;
    public float currentTime;
    
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
        }
        
        if (currentTime/lengthOfDayInSeconds > becomeNightAt && !_night)
        {
            _night = true;
            icon.sprite = moonSprite;
            timeOfDay.ShowText("Night");
        }
    }
}