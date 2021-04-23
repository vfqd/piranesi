using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils.UiAnimation.Interfaces;

namespace Utils.UiAnimation
{
    public class ShakeOnHover : RectTransformTweenerBase, IOnPointerEnter
    {
        public void OnPointerEnter(PointerEventData pointerEventData)
        {
            Sequence anim = DOTween.Sequence();
            anim.Append(toAnimate.DOShakeRotation(.4f, 5f, 20).SetEase(Ease.InOutBounce));
            anim.Append(toAnimate.DOLocalRotate(Vector3.zero, .1f));
            anim.Play();
        }
    }
}