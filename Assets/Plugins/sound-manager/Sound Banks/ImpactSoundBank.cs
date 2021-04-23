using Framework;
using UnityEngine;

namespace SoundManager
{
    [CreateAssetMenu(fileName = "New Impact Bank", menuName = "Sound Bank/Impact Bank")]
    public class ImpactSoundBank : SingleSoundBank
    {

        [SerializeField]
        [Clamp(0, Mathf.Infinity)]
        [Tooltip("Scales the volume of the sound based on the magnitude of the impact velocity.\n\nMin: Impacts below this velocity will be 0% volume.\nMax: Impacts above this velocity will be 100% volume.")]
        private FloatRange _velocityRange = new FloatRange(2f, 5f);

        public ImpactSoundInstance Fetch(ISoundPool soundPool, float impactVelocity)
        {
            ImpactSoundInstance sound = soundPool.FetchFromPool<ImpactSoundInstance>();

            sound.name = name;
            sound.SetClip(GetNextClip());
            sound.SetImpactVelocity(impactVelocity);
            sound.SetVelocityRange(_velocityRange.Min, _velocityRange.Max);
            sound.SetRolloffDistance(_rolloffDistance.Min, _rolloffDistance.Max);
            sound.SetMixerGroup(_outputMixer);
            sound.SetBaseVolume(_volumeRange.ChooseRandom());
            sound.SetBasePitch(_pitchRange.ChooseRandom());

            AddFilters(sound);

            return sound;
        }

#if UNITY_EDITOR
        public override SoundInstance TestInEditor(ISoundPool soundPool)
        {
            ImpactSoundInstance sound = Fetch(soundPool, _velocityRange.ChooseRandom());

            if (sound != null && (_cooldown <= 0 || TimeSinceLastPlayed > _cooldown))
            {
                sound.Play2D();
                OnPlayed(sound);

                return sound;
            }

            return null;
        }
#endif

        [ContextMenu("Apply Default Values")]
        public override void ApplyDefaultValues()
        {
            _volumeRange = SoundManagerSettings.Instance.ImpactBank.VolumeRange;
            _pitchRange = SoundManagerSettings.Instance.ImpactBank.PitchRange;
            _rolloffDistance = SoundManagerSettings.Instance.ImpactBank.RolloffDistance;
            _velocityRange = SoundManagerSettings.Instance.ImpactBank.VelocityRange;
            _cooldown = SoundManagerSettings.Instance.ImpactBank.Cooldown;
            _outputMixer = SoundManagerSettings.Instance.ImpactBank.OutputMixer;
            _clipSelection = SoundManagerSettings.Instance.ImpactBank.ClipSelection;
        }


    }

    public static class ImpactSoundBankExtenstions
    {
        /// <summary>
        /// Fetches and then plays a non-spatial sound, ie. one that does not emit from a specific location and rolloff.
        /// </summary>
        /// <param name="soundBank">The sound bank used to set-up the sound instance</param>
        public static EffectSoundInstance Play(this ImpactSoundBank soundBank, float velocity)
        {
            if (soundBank != null && soundBank.TimeSinceLastPlayed > soundBank.Cooldown)
            {
                EffectSoundInstance sound = soundBank.Fetch(RuntimeSoundPool.Instance, velocity);
                sound.Play2D();
                soundBank.OnPlayed(sound);

                return sound;
            }

            return null;
        }

        /// <summary>
        /// Fetches and then plays a sound at a specific position.
        /// </summary>
        /// <param name="position">The world-space position of the sound emission</param>
        public static EffectSoundInstance Play(this ImpactSoundBank soundBank, Vector3 position, float velocity)
        {
            if (soundBank != null && (soundBank.Cooldown <= 0 || soundBank.TimeSinceLastPlayed > soundBank.Cooldown))
            {
                EffectSoundInstance sound = soundBank.Fetch(RuntimeSoundPool.Instance, velocity);
                sound.Play3D(position);
                soundBank.OnPlayed(sound);

                return sound;
            }

            return null;
        }

        /// <summary>
        /// Fetches and then plays a sound that will follow a transform around.
        /// </summary>
        /// <param name="followTransform">The transform to follow</param>
        public static EffectSoundInstance Play(this ImpactSoundBank soundBank, Transform followTransform, float velocity)
        {
            if (soundBank != null && (soundBank.Cooldown <= 0 || soundBank.TimeSinceLastPlayed > soundBank.Cooldown))
            {
                EffectSoundInstance sound = soundBank.Fetch(RuntimeSoundPool.Instance, velocity);
                sound.Play3D(followTransform);
                soundBank.OnPlayed(sound);

                return sound;
            }

            return null;
        }
    }
}