using DG.Tweening;
using Framework;
using UnityEngine;
using Utils.UiAnimation;
using Utils.UiAnimation.Interfaces;

namespace Utils.UiAnimation.Animations.OnShow
{
    public class GrowOnShow : RectTransformTweenerBase, IOnShow
    {
        public float duration = 0.5f;
        public Ease EaseType = Ease.OutBack;
        
        public void OnShow()
        {
            toAnimate.gameObject.SetActive(true);
            toAnimate.SetScale(0);
            if (siblingDelay)
            {
                toAnimate.DOKill();
                toAnimate.DOScale(new Vector3(1, 1, 1), duration).SetEase(EaseType).SetDelay(siblingDelay.Delay).OnComplete(() =>
                {
                    toAnimate.SetScale(1);
                });
            }
            else
            {
                toAnimate.DOKill();
                toAnimate.DOScale(new Vector3(1, 1, 1), duration).SetEase(EaseType).OnComplete(() =>
                {
                    toAnimate.SetScale(1);
                });
            }
        }
    }
}