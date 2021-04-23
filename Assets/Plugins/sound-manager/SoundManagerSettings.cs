using System;
using Framework;
using UnityEngine;
using UnityEngine.Audio;

namespace SoundManager
{
    public class SoundManagerSettings : ProjectSettingsObject<SoundManagerSettings>
    {

        [Serializable]
        public class EffectSoundBankData
        {
            [Clamp]
            public FloatRange VolumeRange = new FloatRange(0.5f, 0.5f);
            public FloatRange PitchRange = new FloatRange(1f, 1f);
            [MinValue(0)]
            public FloatRange RolloffDistance = new FloatRange(1, 10);
            [MinValue(0)]
            public float Cooldown = 0;
            public AudioMixerGroup OutputMixer;
            public SingleSoundBank.ClipSelectionMode ClipSelection = SingleSoundBank.ClipSelectionMode.Random;
        }

        [Serializable]
        public class ImpactSoundBankData
        {
            [Clamp]
            public FloatRange VolumeRange = new FloatRange(0.5f, 0.5f);
            public FloatRange PitchRange = new FloatRange(1f, 1f);
            [MinValue(0)]
            public FloatRange RolloffDistance = new FloatRange(1, 10);
            [MinValue(0)]
            public FloatRange VelocityRange = new FloatRange(2f, 5f);
            [MinValue(0)]
            public float Cooldown = 0;
            public AudioMixerGroup OutputMixer;
            public SingleSoundBank.ClipSelectionMode ClipSelection = SingleSoundBank.ClipSelectionMode.Random;
        }

        [Serializable]
        public class AmbienceSoundBankData
        {
            public AudioMixerGroup OutputMixer;
            public AmbienceSoundBank.EffectData.SpatialMode EffectSpawnMode = AmbienceSoundBank.EffectData.SpatialMode.XZHemisphere;
            [MinValue(0)]
            public FloatRange EffectSpawnDistance = new FloatRange(5, 10);
            [MinValue(0)]
            public FloatRange EffectCooldown = new FloatRange(1f, 5f);
            [MinValue(0)]
            public FloatRange LoopRolloffDistance = new FloatRange(1, 10);
            [Clamp]
            public FloatRange LoopVariationVolumeRange = new FloatRange(0.5f, 0.5f);
            public float LoopVaritaionFrequency = 0.1f;
        }


        [Serializable]
        public class BlendSoundBankData
        {
            public AudioMixerGroup OutputMixer;
            [MinValue(0)]
            public FloatRange LayerRolloffDistance = new FloatRange(1, 10);
            public AnimationCurve LayerBlendCurve = AnimationCurve.Linear(0, 0, 1, 1);
            public bool LayerLooping = false;
        }


        [Serializable]
        public class SequenceSoundBankData
        {
            public AudioMixerGroup OutputMixer;
            [MinValue(0)]
            public FloatRange SectionRolloffDistance = new FloatRange(1, 10);
            public bool SectionLooping = false;
            [MinValue(1)]
            public int SectionRepititions = 1;
            public float SectionDelay = 0;
        }


        public EffectSoundBankData EffectBank => _effectBank;
        public ImpactSoundBankData ImpactBank => _impactBank;
        public AmbienceSoundBankData AmbienceBank => _ambienceBank;
        public BlendSoundBankData BlendBank => _blendBank;
        public SequenceSoundBankData SequenceBank => _sequenceBank;

        [SerializeField]
        private EffectSoundBankData _effectBank;

        [SerializeField]
        private ImpactSoundBankData _impactBank;

        [SerializeField]
        private AmbienceSoundBankData _ambienceBank;

        [SerializeField]
        private BlendSoundBankData _blendBank;

        [SerializeField]
        private SequenceSoundBankData _sequenceBank;

#if UNITY_EDITOR
        [UnityEditor.SettingsProvider]
        public static UnityEditor.SettingsProvider CreateSettingsProvider()
        {
            return CreateSettingsProviderInternal();
        }
#endif
    }
}
