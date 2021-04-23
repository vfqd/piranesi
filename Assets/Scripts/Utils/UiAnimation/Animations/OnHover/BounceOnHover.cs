using System;
using DG.Tweening;
using Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils.UiAnimation.Interfaces;

namespace Utils.UiAnimation
{
    public class BounceOnHover : RectTransformTweenerBase, IOnPointerEnter
    {
        public void OnPointerEnter(PointerEventData pointerEventData)
        {
            toAnimate.DOPunchScale(new Vector3(.05f,.05f), .5f).SetEase(Ease.InOutBounce).OnComplete(() => toAnimate.SetScale(1));
        }
    }
}

