using System;
using Buildings;
using Researches;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class BuildingButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Building building;
    [TextArea]
    public string description;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        HoverPanel.Instance.ShowText(building.name +
                                     "\n" +
                                     (building.woodCost > 0 ? building.woodCost + " lumber" + "\n" : "") +
                                     (building.metalCost > 0 ? building.metalCost + " metals" + "\n" : "") +
                                     "\n" +
                                     description
        );
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HoverPanel.Instance.HideText();
    }

    private void Update()
    {
        _button.interactable = true;

        if (building.woodCost > 0 && ResourcesController.Instance.woodCount < building.woodCost)
        {
            _button.interactable = false;
        }
        
        if (building.metalCost > 0 && ResourcesController.Instance.metalsCount < building.metalCost)
        {
            _button.interactable = false;
        }
    }
}
