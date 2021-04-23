using DG.Tweening;
using UnityEngine;
using Utils.UiAnimation;
using Utils.UiAnimation.Interfaces;

namespace Utils.UiAnimation.Animations.OnHide
{
    public class ShrinkOnHide : RectTransformTweenerBase, IOnHide
    {
        public void OnHide()
        {
            toAnimate.DOKill();
            
            if (siblingDelay)
            {
                toAnimate.DOScale(Vector3.zero, .25f).SetEase(Ease.OutCirc).SetDelay(siblingDelay.Delay);
                toAnimate.gameObject.SetActive(false);
            }
            else
            {
                toAnimate.DOScale(Vector3.zero, .25f).SetEase(Ease.OutCirc);
                toAnimate.gameObject.SetActive(false);
            }
        }
    }
}