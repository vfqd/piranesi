using System;
using DG.Tweening;
using UnityEngine;
using Utils;

namespace Buildings
{
    public class Portal : MonoSingleton<Portal>
    {
        private void OnMouseOver()
        {
            string s = "Produces: 1 food, 1 lumber, and 1 metals per day";
            HoverPanel.Instance.ShowText(s);
        }

        private void OnMouseExit()
        {
            HoverPanel.Instance.HideText();
        }

        public void ProduceResources()
        {
            if (!GameController.Instance.hasLeftLondon) return;
            
            ResourcesController.Instance.foodCount++;
            ResourcesController.Instance.woodCount++;
            ResourcesController.Instance.metalsCount++;
            transform.DOPunchScale(Vector3.one * .1f, .33f, 1, .1f);

        }
    }
}