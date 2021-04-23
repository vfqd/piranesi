using Framework;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;

namespace SoundManager
{
    /// <summary>
    /// Instance of a sound from the SoundPool pool. Basically an AudioSource setup to play a EffectSoundBank.
    /// </summary>
    public class EffectSoundInstance : SoundInstance
    {

        public AudioClip Clip
        {
            get => _audioSource.clip;
            set => SetClip(value);
        }

        public float Time
        {
            get => _audioSource.time / _audioSource.pitch;
            set => SetTime(value);
        }
        public float NormalizedTime
        {
            get => Time / Length;
            set => SetNormalizedTime(value);
        }
        public float Length
        {
            get => _audioSource.clip == null ? 0 : _audioSource.clip.length / _audioSource.pitch;
            set => SetLength(value);
        }
        public float TimeRemaining
        {
            get => Length - Time;
            set => SetTimeRemaining(value);
        }

        public bool IsLooping
        {
            get => _audioSource.loop;
            set => SetLooping(value);
        }

        public FloatRange RolloffDistance
        {
            get => new FloatRange(_audioSource.minDistance, _audioSource.maxDistance);
            set => SetRolloffDistance(value.Min, value.Max);
        }

        /// <summary>
        /// The base linear volume level to play the sound at. This value will be modified by the volume multiplier and any fades before finally being converted to logarithmic volume and used by the AudioSource.
        /// </summary>
        public float BaseVolume
        {
            get => _baseVolume;
            set => SetBaseVolume(value);
        }
        /// <summary>
        /// The base pitch level to play the sound at. This value will be modified by the picth multiplier before being used by the AudioSource.
        /// </summary>
        public float BasePitch
        {
            get => _basePitch;
            set => SetBasePitch(value);
        }
        /// <summary>
        /// The AudioSource that will actually play the sound.
        /// </summary>
        public AudioSource AudioSource => _audioSource;

        protected AudioSource _audioSource;
        protected float _baseVolume = 0.5f;
        protected float _basePitch = 1f;

        protected bool _hasStartedPlaying;

        public override void Reset()
        {
            base.Reset();

            if (_audioSource == null)
            {
                _audioSource = gameObject.GetOrAddComponent<AudioSource>();
            }

            _hasStartedPlaying = false;
            _pitchMultiplier = 1f;
            _baseVolume = 0.5f;
            _basePitch = 1f;

            _audioSource.Stop();
            _audioSource.clip = null;
            _audioSource.time = 0f;
            _audioSource.playOnAwake = false;
            _audioSource.volume = 0.5f;
            _audioSource.pitch = 1f;
            _audioSource.loop = false;
            _audioSource.spatialBlend = 1f;
            _audioSource.priority = 128;
            _audioSource.panStereo = 0f;
            _audioSource.reverbZoneMix = 1f;
            _audioSource.dopplerLevel = 1f;
            _audioSource.spread = 0f;
            _audioSource.outputAudioMixerGroup = null;
            _audioSource.rolloffMode = AudioRolloffMode.Linear;
            _audioSource.minDistance = 1;
            _audioSource.maxDistance = 500;
            _audioSource.mute = false;
            _audioSource.bypassEffects = false;
            _audioSource.bypassListenerEffects = false;
            _audioSource.bypassReverbZones = false;
        }

        public override void UpdateSound(float deltaTime)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            base.UpdateSound(deltaTime);

            if (_positionMode == PositionMode.FollowingTransform)
            {
                if (_followTransform == null)
                {
                    StopAndDestroy();
                }
                else
                {
                    transform.position = _followTransform.position;
                }
            }

            if (_state == State.Playing)
            {
                if (_delayTimer <= 0 && !_hasStartedPlaying && _audioSource.clip != null)
                {
                    _delayTimer = 0;
                    _audioSource.spatialBlend = _positionMode == PositionMode.TwoDimensional ? 0f : 1f;
                    _audioSource.Play();
                    _hasStartedPlaying = true;
                }

                if (_delayTimer > 0)
                {
                    _delayTimer -= deltaTime;
                }

                _audioSource.pitch = GetCurrentPitch();
                _audioSource.volume = MathUtils.GetLogarithmicVolume(GetCurrentVolume());

                if (_audioSource.clip == null || (_hasStartedPlaying && !_audioSource.isPlaying && !_audioSource.loop))
                {
                    Finish();
                }

            }

            if (HasFinished && _autoDestroy)
            {
                StopAndDestroy();
            }
        }

        public override void AddFilter(SoundBankFilter filter)
        {
            base.AddFilter(filter);

            if (filter.IsEnabled)
            {
                filter.OnAdded(this);
            }
        }

        public override void RemoveFilters()
        {
            for (int i = 0; i < _filters.Count; i++)
            {
                _filters[i].OnRemoved(this);
            }

            base.RemoveFilters();
        }

        /// <summary>
        /// The current pitch to play at, which is simply the base pitch multiplied by the pitch multiplier.
        /// </summary>
        /// <returns>The currently playing pitch</returns>
        public virtual float GetCurrentPitch()
        {
            float filterMultiplier = 1f;

            if (_audioSource != null && _audioSource.clip != null)
            {
                for (int i = 0; i < _filters.Count; i++)
                {
                    filterMultiplier *= _filters[i].GetPitchMultiplier(_audioSource.time / _audioSource.pitch, _audioSource.clip.length / _audioSource.pitch);
                }
            }

            return _basePitch * _pitchMultiplier * _parentPitchMultiplier * filterMultiplier;
        }

        /// <summary>
        /// The current linear volume to play at, which is base volume multiplied by the volume multiplier and modified by any fades.
        /// </summary>
        /// <returns>The currently playing linear volume</returns>
        public virtual float GetCurrentVolume()
        {
            float filterMultiplier = 1f;

            if (_audioSource != null && _audioSource.clip != null)
            {
                for (int i = 0; i < _filters.Count; i++)
                {
                    filterMultiplier *= _filters[i].GetVolumeMultiplier(_audioSource.time / _audioSource.pitch, _audioSource.clip.length / _audioSource.pitch);
                }
            }

            return Mathf.Clamp01(_baseVolume * _volumeMultiplier * _parentVolumeMultiplier * filterMultiplier);
        }

        /// <summary>
        /// Stops playing the sound. If the sound instance is not reserved, this will return it to the pool. If it is reseverd, it will be reset and ready to be played from the beginning.
        /// </summary>
        public override SoundInstance StopAndDestroy()
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            _audioSource.Stop();

            return base.StopAndDestroy();
        }

        /// <summary>
        /// Pauses playing the sound.
        /// </summary>
        public override void Pause()
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            if (_state == State.Playing)
            {
                _hasStartedPlaying = false;
                _audioSource.Pause();
                _state = State.Paused;
            }
        }

        /// <summary>
        /// Resumes playing the paused sound.
        /// </summary>
        public override void Resume()
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            if (_state == State.Paused)
            {
                // _playHasStarted = false;
                _state = State.Playing;
            }
        }


        public EffectSoundInstance SetClip(AudioClip clip)
        {
            _audioSource.clip = clip;

            return this;
        }

        public override SoundInstance SetMixerGroup(AudioMixerGroup mixerGroup)
        {
            base.SetMixerGroup(mixerGroup);

            _audioSource.outputAudioMixerGroup = mixerGroup;

            return this;
        }

        /// <summary>
        /// Sets the base linear volume level to play the sound at. This value will be modified by the volume multiplier and any fades before finally being converted to logarithmic volume and used by the AudioSource.
        /// </summary>
        /// <param name="volume">The linear base volume to play at</param>
        public EffectSoundInstance SetBaseVolume(float volume)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            _baseVolume = Mathf.Clamp01(volume);
            _audioSource.volume = MathUtils.GetLogarithmicVolume(GetCurrentVolume());

            return this;
        }

        public override SoundInstance SetVolumeMultiplier(float multiplier)
        {
            base.SetVolumeMultiplier(multiplier);
            _audioSource.volume = MathUtils.GetLogarithmicVolume(GetCurrentVolume());

            return this;
        }

        /// <summary>
        /// Sets the base pitch level to play the sound at. This value will be modified by the picth multiplier before being used by the AudioSource.
        /// </summary>
        /// <param name="pitch">The base pitch level to play at</param>
        public EffectSoundInstance SetBasePitch(float pitch)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            _basePitch = pitch;
            _audioSource.pitch = GetCurrentPitch();

            return this;
        }

        public override SoundInstance SetPitchMultiplier(float multiplier)
        {
            base.SetPitchMultiplier(multiplier);
            _audioSource.pitch = GetCurrentPitch();

            return this;
        }

        /// <summary>
        /// Sets whether or not the sound play again when it has finished.
        /// </summary>
        /// <param name="looping">Whether or not the sound should loop</param>
        public EffectSoundInstance SetLooping(bool looping)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            _audioSource.loop = looping;

            return this;
        }

        public EffectSoundInstance SetRolloffDistance(float minDistance, float maxDistance)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            _audioSource.minDistance = Mathf.Max(0, minDistance);
            _audioSource.maxDistance = Mathf.Max(0, maxDistance);

            return this;
        }



        /// <summary>
        /// Resets the sound so that it is ready to be played from the beginning.
        /// </summary>
        public override void Rewind()
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            base.Rewind();

            _audioSource.Stop();
            _audioSource.time = 0f;
            _delayTimer = _delayDuration;
            _hasStartedPlaying = false;
        }

        public EffectSoundInstance SetLength(float length)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");
            Assert.IsNotNull(_audioSource.clip);

            SetBasePitch(_audioSource.clip.length / length);

            return this;
        }

        public EffectSoundInstance SetTimeRemaining(float time, bool adjustByPitch = true)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            SetTime(Length - time, adjustByPitch);

            return this;
        }

        /// <summary>
        /// Sets the time (in seconds) that the sound should play from. Useful for jumping or scrubbing to a specfic part of a sound clip.
        /// </summary>
        /// <param name="time">The time (in seconds) to play from</param>
        /// <param name="adjustByPitch">Whether or not to adjust the supplied time value to account for a non-standard pitch level</param>
        public EffectSoundInstance SetTime(float time, bool adjustByPitch = true)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            if (adjustByPitch)
            {
                _audioSource.time = Mathf.Max(0, time) * _audioSource.pitch;
            }
            else
            {
                _audioSource.time = Mathf.Max(0, time);
            }

            return this;
        }

        /// <summary>
        /// Sets the normalized time that the sound should play from. Useful for jumping or scrubbing to a specfic part of a sound clip.
        /// </summary>
        /// <param name="time">The normalized time to play from/param>
        /// <param name="adjustByPitch">Whether or not to adjust the supplied time value to account for a non-standard pitch level</param>
        public EffectSoundInstance SetNormalizedTime(float time, bool adjustByPitch = true)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            if (_audioSource.clip != null)
            {
                if (adjustByPitch)
                {
                    _audioSource.time = Mathf.Lerp(0, _audioSource.clip.length, Mathf.Clamp01(time)) * _audioSource.pitch;
                }
                else
                {
                    _audioSource.time = Mathf.Lerp(0, _audioSource.clip.length, Mathf.Clamp01(time));
                }
            }

            return this;
        }


        public override string GetStatusString()
        {
            if (_audioSource.clip == null)
            {
                return "No clip";
            }

            return _audioSource.clip.name + " (Volume: " + BaseVolume.ToString("F2") + ", Pitch: " + BasePitch.ToString("F2") + ")";
        }
    }
}

