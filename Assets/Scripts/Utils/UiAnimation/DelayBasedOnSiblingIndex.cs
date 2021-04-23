using UnityEngine;

namespace Utils.UiAnimation
{
    public class DelayBasedOnSiblingIndex : MonoBehaviour
    {
        public float Delay => transform.GetSiblingIndex() * .2f;
    }
}