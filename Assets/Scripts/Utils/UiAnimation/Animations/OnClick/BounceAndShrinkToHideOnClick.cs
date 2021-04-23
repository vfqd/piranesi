using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils.UiAnimation.Interfaces;

namespace Utils.UiAnimation.Animations.OnClick
{
    public class BounceAndShrinkToHideOnClick : RectTransformTweenerBase, IOnClick
    {
        public void OnClick(PointerEventData eventData)
        {
            toAnimate.DOScale(Vector3.zero, .25f).SetEase(Ease.InElastic);
        }
    }
}