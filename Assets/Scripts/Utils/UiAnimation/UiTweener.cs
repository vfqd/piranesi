using System;
using System.Collections.Generic;
using NaughtyAttributes;
using OneLine;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils.UiAnimation.Interfaces;

namespace Utils.UiAnimation
{
    public class UiTweener : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
    {
        public bool startsHidden;
        public bool useOnPointerEnterEvent = true;
        public bool useOnPointerClickEvent = true;

        public bool IsHidden => _isHidden;

        private bool _isHidden;
        private bool _isQuitting;

        private void Start()
        {
            if (startsHidden)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_isHidden && useOnPointerEnterEvent) foreach (var anim in GetComponents<IOnPointerEnter>()) anim.OnPointerEnter(eventData);
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_isHidden && useOnPointerClickEvent) foreach (var anim in GetComponents<IOnClick>()) anim.OnClick(eventData);
        }

        public void Show()
        {
            if (_isHidden)
            {
                _isHidden = false;
                foreach (var anim in GetComponents<IOnShow>()) anim.OnShow();
            }
        }
        
        public void Hide()
        {
            if (!_isHidden)
            {
                _isHidden = true;
                foreach (var anim in GetComponents<IOnHide>()) anim.OnHide();  
            }
        }
        
        public void HideDelayed(float delay)
        {
            Invoke(nameof(Hide),delay);
        }

        public void CancelHide()
        {
            if (IsInvoking(nameof(Hide))) { CancelInvoke(nameof(Hide)); }
        }

        private void OnApplicationQuit()
        {
            _isQuitting = true;
        }
    }
}