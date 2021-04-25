using System;
using UnityEngine;
using UnityEngine.UI;

namespace Researches
{
    public class Research : MonoBehaviour
    {
        public string name;
        [TextArea] public string description;
        [TextArea] public string textWhenResearched;
        public int cost;
        public bool isVisible;
        public bool isCompleted;
        public int progress;

        private GameObject _notUnlockedIcon;
        private Button _button;

        [HideInInspector] public Image iconRenderer;

        private void Awake()
        {
            _notUnlockedIcon = transform.Find("NotUnlockedIcon").gameObject;
            iconRenderer = transform.Find("Icon").GetComponent<Image>();
            _button = GetComponent<Button>();
        }

        private void Update()
        {
            _notUnlockedIcon.SetActive(!isVisible);
            _button.interactable = isVisible;
        }

        public void IncreaseProgress(int amount)
        {
            if (progress >= cost) return;
            
            progress += amount;
            if (progress >= cost)
            {
                isCompleted = true;
                EffectOnComplete();
            }
        }
        
        protected virtual void EffectOnComplete()
        {
            DialogController.Instance.PlayDialog(textWhenResearched);
        }
    }
}