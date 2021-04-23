using DG.Tweening;
using Framework;
using UnityEngine;
using Utils.UiAnimation;
using Utils.UiAnimation.Interfaces;

namespace Utils.UiAnimation.Animations.OnHide
{
    public class BounceAndShrinkOnHide : RectTransformTweenerBase, IOnHide
    {
        public void OnHide()
        {
            toAnimate.DOKill();
            toAnimate.DOScale(Vector3.zero, .1f).SetEase(Ease.InBack).OnComplete(() =>
            {
                toAnimate.SetScale(0);
                toAnimate.gameObject.SetActive(false);
            });
        }
    }
}