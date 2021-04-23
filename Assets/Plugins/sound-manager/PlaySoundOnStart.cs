using Framework;
using UnityEngine;

namespace SoundManager
{
    public class PlaySoundOnStart : MonoBehaviour
    {
        public enum SoundPosition
        {
            TwoDimensional,
            TransformPosition,
            FollowTransform
        }

        [SerializeField]
        private SoundBank _soundBank;

        [SerializeField]
        private SoundPosition _position;

        [SerializeField]
        private bool _loop;

        [SerializeField]
        [MinValue(0)]
        private float _fadeInDuration = 0;

        void Start()
        {
            SoundInstance sound = null;

            switch (_position)
            {
                case SoundPosition.TwoDimensional:
                    sound = _soundBank.Play();
                    break;
                case SoundPosition.TransformPosition:
                    sound = _soundBank.Play(transform.position);
                    break;
                case SoundPosition.FollowTransform:
                    sound = _soundBank.Play(transform);
                    break;
            }

            if (_loop)
            {
                EffectSoundInstance effectSound = sound as EffectSoundInstance;
                if (effectSound != null)
                {
                    effectSound.SetLooping(true);
                }
            }

            if (sound != null && _fadeInDuration > 0)
            {
                sound.FadeIn(_fadeInDuration, true);
            }
        }

    }
}
