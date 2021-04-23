using System;
using Framework;
using Shapes;
using UnityEngine;

namespace Utils
{
    [ExecuteInEditMode] [RequireComponent(typeof(RectTransform),typeof(ShapeRenderer))]
    public class LinkShapeToRectTransform : MonoBehaviour
    {
        private ShapeRenderer _shape;
        private RectTransform _rectTransform;

        private void Awake()
        {
            _shape = GetComponent<ShapeRenderer>();
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (_shape is Rectangle rectangle)
            {
                rectangle.Width = _rectTransform.GetLocalWidth();
                rectangle.Height = _rectTransform.GetLocalHeight();
            }
        }
    }
}