using System;
using Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class HoverPanel : MonoSingleton<HoverPanel>
{
    public TextMeshProUGUI description;

    private RectTransform _rectTransform;

    public static bool IsHovering = false;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        HideText();
    }

    public void ShowText(string s)
    {
        GetComponent<Image>().enabled = true;
        description.text = s;
        IsHovering = true;
    }

    public void HideText()
    {
        GetComponent<Image>().enabled = false;
        description.text = "";
        IsHovering = false;
    }

    private void Update()
    {
        Vector2 offset = new Vector2(96, 0);
        
        if (Input.mousePosition.y < Screen.height * 0.2f)
        {
            _rectTransform.anchorMin = Vector2.zero;
            _rectTransform.anchorMax = Vector2.zero;
            offset.y = 32;
        }
        else if (Input.mousePosition.y > Screen.height * 0.8f)
        {
            _rectTransform.anchorMin = new Vector2(0,1);
            _rectTransform.anchorMax = new Vector2(0,1);
            offset.y = -32;
        }
        else
        {
            _rectTransform.anchorMin = new Vector2(0,.5f);
            _rectTransform.anchorMax = new Vector2(0,.5f);
        }

        transform.position = Input.mousePosition + offset.ToVector3();
    }
}