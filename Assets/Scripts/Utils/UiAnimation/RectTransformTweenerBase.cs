using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utils.UiAnimation
{
    public class RectTransformTweenerBase : MonoBehaviour
    {
        public RectTransform toAnimate;
        protected DelayBasedOnSiblingIndex siblingDelay;

        private void Awake()
        {
            siblingDelay = GetComponent<DelayBasedOnSiblingIndex>();
        }
    }
}