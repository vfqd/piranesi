using Framework;
using UnityEngine;
using Utils.UiAnimation;

namespace Utils
{
    [ExecuteInEditMode] [RequireComponent(typeof(RectTransform))]
    public class ScaleWidthWithChildCount : MonoBehaviour
    {
        [SerializeField] private float padding;
        [SerializeField] private float sizePerChild;

        private RectTransform _rectTransform;
        
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }
        
        private void Update()
        {
            int enabledChildren = 0;
            foreach (Transform child in transform) if (child.gameObject.activeInHierarchy && child.GetComponent<UiTweener>() && !child.GetComponent<UiTweener>().IsHidden) enabledChildren++;
            float targetWidth = sizePerChild*enabledChildren + padding*2;
            if (_rectTransform.GetLocalWidth() < targetWidth)
            {
                _rectTransform.SetLocalWidth(_rectTransform.GetLocalWidth()+padding/2);
            }
            else if (_rectTransform.GetLocalWidth() > targetWidth)
            {
                _rectTransform.SetLocalWidth(_rectTransform.GetLocalWidth()-padding/2);
            }
        }
    }
}