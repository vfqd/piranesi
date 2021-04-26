using System;
using Buildings;
using UnityEngine;

public class BuildingHover : MonoBehaviour
{
    private Building _building;

    private void Awake()
    {
        _building = GetComponent<Building>();
    }

    private void OnMouseEnter()
    {
        if (!_building || !_building.placed) return;
        if (ResearchController.Instance.panel.activeInHierarchy) return;
        
        string s = _building.acolyteCount + "/" + _building.acolytesMax + " acolytes.";

        if (_building.type == Building.Type.Altar)
        {
            s += "\nProduces " + GetComponent<Altar>().GetResearchProducedCount() + " Knowledge per day";
        }
        else if (_building.type == Building.Type.Lumbercamp)
        {
            s += "\nProduces " + GetComponent<Lumbermill>().GetLumberProducedCount() + " lumber per day";
        }
        else if (_building.type == Building.Type.Mine)
        {
            s += "\nProduces " + GetComponent<Mine>().GetMetalProducedCount() + " metals per day";
        }
        else if (_building.type == Building.Type.Fishery)
        {
            s += "\nProduces " + GetComponent<Wharf>().GetFoodProducedCount() + " food per day";
        }
        else if (_building.type == Building.Type.Pen)
        {
            s = _building.acolyteCount + "/" + _building.acolytesMax + " prisoners.";
        }
        
        HoverPanel.Instance.ShowText(s);
    }

    private void OnMouseExit()
    {
        HoverPanel.Instance.HideText();
    }
}