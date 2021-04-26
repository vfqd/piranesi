using System;
using SoundManager;
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
        public bool repeatable;
        public int progress;
        public Research[] toEnableNext;
        public bool dontAllowIfNoSpace;

        private GameObject _notUnlockedIcon;
        private GameObject _completeIcon;
        private Button _button;
        
        public EffectSoundBank effectSoundBank;


        [HideInInspector] public Image iconRenderer;

        private void Awake()
        {
            _notUnlockedIcon = transform.Find("NotUnlockedIcon").gameObject;
            _completeIcon = transform.Find("CompleteIcon").gameObject;
            iconRenderer = transform.Find("Icon").GetComponent<Image>();
            _button = GetComponent<Button>();
        }

        private void Update()
        {
            _notUnlockedIcon.SetActive(!isVisible);
            _button.interactable = isVisible && !isCompleted;

            if (dontAllowIfNoSpace)
            {
                if (ResourcesController.Instance.acolytesCount >= ResourcesController.Instance.acolytesMax)
                {
                    _button.interactable = false;
                }
            }
            
            _completeIcon.SetActive(isCompleted);
        }

        public bool IncreaseProgress(int amount)
        {
            progress += amount;
            if (progress >= cost)
            {
                EffectOnComplete();
                if (repeatable)
                {
                    progress = 0;
                    cost += 5;
                }
                else
                {
                    isCompleted = true;
                }
                return true;
            }
            return false;
        }
        
        protected virtual void EffectOnComplete()
        {
            DialogController.Instance.PlayDialog(textWhenResearched);
            effectSoundBank.Play();

            foreach (var research in toEnableNext)
            {
                if (research) research.isVisible = true;
            }
        }
    }
}