using System;
using Framework;
using UnityEngine;

namespace Buildings
{
    public class Compound : MonoBehaviour
    {
        private Building _building;

        private void Awake()
        {
            _building = GetComponent<Building>();
        }

        public void TakeCosts()
        {
            if (_building.acolyteCount > 0 && ResourcesController.Instance.foodCount > 0)
            {
                ResourcesController.Instance.foodCount--;
            }
            else
            {
                if (_building.acolyteCount > 0)
                {
                    _building.RemoveAcolyte();
                    DialogController.Instance.PlayDialog("An acolyte has died of starvation.");

                    var buildings = MapController.Instance.buildings;
                    buildings.Shuffle();
                    foreach (var building in buildings)
                    {
                        if (building.type == Building.Type.Altar || 
                             building.type == Building.Type.Watchpost || 
                             building.type == Building.Type.Fishery ||
                             building.type == Building.Type.Lumbercamp ||
                             building.type == Building.Type.Mine)
                        {
                            building.RemoveAcolyte();
                            ResourcesController.Instance.acolytesCount--;
                            break;
                        }
                    }
                }
            }
        }
    }
}