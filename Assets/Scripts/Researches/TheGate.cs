using DG.Tweening;
using Febucci.UI;
using SoundManager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Researches
{
    public class TheGate : Research
    {
        public Image fullscreenImage;
        public TextMeshProUGUI text;
        public Image quitButton;

        public EffectSoundBank boomSound;
        public EffectSoundBank writeSound;

        public Transform gateTransform;

        public void Complete()
        {
            EffectOnComplete();
        }
        
        protected override void EffectOnComplete()
        {
            base.EffectOnComplete();
            fullscreenImage.gameObject.SetActive(true);
            fullscreenImage.color = Color.clear;
            CameraController.Instance.transform.position = new Vector3(41, 26, -10);
            CameraController.Instance.lockPos = true;
            Invoke(nameof(PlaySound),1);
            Invoke(nameof(PlaySound),3);
            Invoke(nameof(PlaySound),5);

            fullscreenImage.DOColor(Color.black, 5).SetEase(Ease.Linear).SetDelay(5);
            Invoke(nameof(StartText),10);
            Invoke(nameof(ShowButton),20);
        }

        private int _multiplier = 1;
        private void PlaySound()
        {
            boomSound.Play().SetVolumeMultiplier(_multiplier);
            
            CameraController.Instance.shaker.ShakeOnce(3*_multiplier+1,2*_multiplier+1,0,_multiplier/2f);
            gateTransform.DOPunchScale(Vector3.one * .1f, .33f, 1, .1f);
            
            _multiplier++;
        }

        private void StartText()
        {
            text.GetComponent<TextAnimatorPlayer>().ShowText("And so, after much hardship, I complete the Great and Necessary Work.");
            writeSound.Play();
        }
        
        private void ShowButton()
        {
            quitButton.gameObject.SetActive(true);
            quitButton.color =Color.clear;
            quitButton.DOColor(Color.white, 5).SetEase(Ease.Linear);
            
            quitButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.clear;
            quitButton.GetComponentInChildren<TextMeshProUGUI>().DOColor(Color.white, 5).SetEase(Ease.Linear);
        }
    }
}