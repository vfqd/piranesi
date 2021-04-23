using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;

namespace SoundManager
{
    public abstract class SoundInstance : MonoBehaviour
    {
        protected internal enum State
        {
            Pooled,
            Fetched,
            Playing,
            Paused
        }

        protected internal enum PositionMode
        {
            TwoDimensional,
            FixedPosition,
            FollowingTransform
        }

        /// <summary>
        /// A mulitplier value that can be used to affect the final playing volume of the sound.
        /// </summary>
        public float VolumeMultiplier
        {
            get => _volumeMultiplier;
            set => SetVolumeMultiplier(value);
        }
        /// <summary>
        /// A mulitplier value that can be used to affect the final playing pitch of the sound.
        /// </summary>
        public float PitchMultiplier
        {
            get => _pitchMultiplier;
            set => SetPitchMultiplier(value);
        }

        public bool HasFinished => _hasFinished;
        public bool IsFadingIn => _fadeSpeed > 0;
        public bool IsFadingOut => _fadeSpeed < 0;

        /// <summary>
        /// Whether or not this sound is currently residing in the pool.
        /// </summary>
        public bool IsPooled => _state == State.Pooled;

        /// <summary>
        /// Whether or not the sound is currently playing. Will return false if the sound is waiting for a delayed play.
        /// </summary>
        public bool IsPlaying => _state == State.Playing;

        /// <summary>
        /// Whether or not the sound is currently paused.
        /// </summary>
        public bool IsPaused => _state == State.Paused;

        public bool IsPersistant
        {
            get => _isPersistant;
            set => SetPersistant(value);
        }
        public AudioMixerGroup MixerGroup
        {
            get => _mixerGroup;
            set => SetMixerGroup(value);
        }
        public bool AutoDestroy
        {
            get => _autoDestroy;
            set => SetAutoDestroy(value);
        }
        public bool IsDelayed => _delayTimer > 0f;
        public float CurrentDelay
        {
            get => _delayTimer;
            set => SetDelay(value);
        }

        protected float _pitchMultiplier = 1f;
        protected float _volumeMultiplier = 1f;
        protected float _parentPitchMultiplier = 1f;
        protected float _parentVolumeMultiplier = 1f;
        protected float _fadeSpeed = 0f;
        protected Transform _followTransform;
        protected PositionMode _positionMode;
        protected bool _isPersistant;
        protected AudioMixerGroup _mixerGroup;
        protected ISoundPool _soundPool;
        protected Action _onFinish;
        protected State _state;
        protected bool _autoDestroy = true;
        protected bool _destroyAtEndOfFade = false;
        protected SoundInstance _parentSound;
        protected List<SoundBankFilter> _filters = new List<SoundBankFilter>();
        protected float _delayTimer;
        protected float _delayDuration;
        private bool _hasFinished;

        public virtual void Reset()
        {
            _delayDuration = 0f;
            _delayTimer = 0f;
            _pitchMultiplier = 1f;
            _volumeMultiplier = 1f;
            _parentPitchMultiplier = 1f;
            _parentVolumeMultiplier = 1f;
            _fadeSpeed = 0f;
            _onFinish = null;
            _isPersistant = false;
            _hasFinished = false;
            _followTransform = null;
            _mixerGroup = null;
            _autoDestroy = true;
            _destroyAtEndOfFade = false;
            _positionMode = PositionMode.TwoDimensional;

            transform.position = Vector3.zero;

            RemoveFilters();
        }

        public void SetParentMultipliers(float volumeMultiplier, float pitchMultiplier)
        {
            _parentVolumeMultiplier = volumeMultiplier;
            _parentPitchMultiplier = pitchMultiplier;
        }

        public virtual void AddFilter(SoundBankFilter filter)
        {
            if (filter.IsEnabled)
            {
                _filters.Add(filter);
            }
        }

        public virtual void RemoveFilters()
        {
            _filters.Clear();
        }

        public void OnFinish(Action callback)
        {
            _onFinish += callback;
        }

        protected void Finish()
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            if (!_hasFinished)
            {
                _hasFinished = true;

                if (_onFinish != null)
                {
                    _onFinish();
                }
            }
        }

        protected internal void Play(PositionMode mode, Transform followTransform, Vector3 position)
        {
            switch (mode)
            {
                case PositionMode.TwoDimensional:
                    Play2D();
                    break;
                case PositionMode.FixedPosition:
                    Play3D(position);
                    break;
                case PositionMode.FollowingTransform:
                    Play3D(followTransform);
                    break;
            }
        }

        public void Play3D(Vector3 position)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            transform.position = position;
            _positionMode = PositionMode.FixedPosition;
            _state = State.Playing;
        }

        public void Play3D(Transform followTransform)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");
            Assert.IsNotNull(followTransform);

            _followTransform = followTransform;
            _positionMode = PositionMode.FollowingTransform;
            _state = State.Playing;
        }

        public void Play2D()
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            _positionMode = PositionMode.TwoDimensional;
            _state = State.Playing;
        }

        public virtual void Rewind()
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            _hasFinished = false;
        }

        protected virtual void LateUpdate()
        {
            UpdateSound(Mathf.Min(Time.unscaledDeltaTime, Time.maximumDeltaTime));
        }

        public virtual void UpdateSound(float deltaTime)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");
            Assert.IsTrue(deltaTime >= 0);

            if (_fadeSpeed != 0f && _destroyAtEndOfFade && (_volumeMultiplier + _fadeSpeed * deltaTime > 1f || _volumeMultiplier + _fadeSpeed * deltaTime < 0f))
            {
                StopAndDestroy();
            }
            else
            {
                for (int i = 0; i < _filters.Count; i++)
                {
                    _filters[i].Update(deltaTime);
                }

                _volumeMultiplier = Mathf.Clamp01(_volumeMultiplier + _fadeSpeed * deltaTime);
            }
        }

        public virtual SoundInstance SetPersistant(bool persistant)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            _isPersistant = persistant;

            return this;
        }

        public virtual SoundInstance SetMixerGroup(AudioMixerGroup mixerGroup)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            _mixerGroup = mixerGroup;

            return this;
        }

        public virtual SoundInstance SetAutoDestroy(bool autoDestroy)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            _autoDestroy = autoDestroy;

            return this;
        }

        public virtual SoundInstance LerpVolumeMultiplier(float target, float speed)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            _volumeMultiplier = Mathf.Lerp(_volumeMultiplier, Mathf.Clamp01(target), Mathf.Min(Time.unscaledDeltaTime, Time.maximumDeltaTime) * speed);

            return this;
        }

        public virtual SoundInstance FadeOut(float fadeDuration, bool startFromOne)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            if (fadeDuration == 0)
            {
                _volumeMultiplier = 0;
            }
            else
            {
                if (startFromOne)
                {
                    _volumeMultiplier = 1f;
                }

                _fadeSpeed = -1f / Mathf.Max(0, fadeDuration);
            }

            return this;
        }

        public virtual SoundInstance FadeIn(float fadeDuration, bool startFromZero)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            if (fadeDuration == 0)
            {
                _volumeMultiplier = 1f;
            }
            else
            {
                if (startFromZero)
                {
                    _volumeMultiplier = 0f;
                }

                _fadeSpeed = 1f / Mathf.Max(0, fadeDuration);
            }

            return this;
        }

        public virtual SoundInstance FadeOutAndDestroy(float fadeDuration, bool startFromOne)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            _destroyAtEndOfFade = true;
            FadeOut(fadeDuration, startFromOne);

            return this;
        }

        public virtual SoundInstance FadeInAndDestroy(float fadeDuration, bool startFromZero)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            _destroyAtEndOfFade = true;
            FadeIn(fadeDuration, startFromZero);

            return this;
        }

        public virtual SoundInstance StopFading()
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            _destroyAtEndOfFade = false;
            _fadeSpeed = 0f;

            return this;
        }

        /// <summary>
        /// Sets the volume multiplier, the base volume will be multiplied by this number when determining the final output volume.
        /// </summary>
        /// <param name="multiplier">The volume multiplier value</param>
        public virtual SoundInstance SetVolumeMultiplier(float multiplier)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            _volumeMultiplier = Mathf.Clamp01(multiplier);

            return this;
        }

        /// <summary>
        /// Sets the pitch multiplier, the base pitch level will be multiplied by this number when determining the final output pitch.
        /// </summary>
        /// <param name="multiplier">The pitch multiplier value</param>
        public virtual SoundInstance SetPitchMultiplier(float multiplier)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            _pitchMultiplier = multiplier;

            return this;
        }

        public virtual SoundInstance SetDelay(float delayDuration)
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            _delayTimer = Mathf.Max(0, delayDuration);
            _delayDuration = Mathf.Max(0, delayDuration);

            return this;
        }

        /// <summary>
        /// Callback for when this sound instance has just been fetched from the pool. This should only ever be called by the SoundPool.
        /// </summary>
        public virtual void OnFetchedFromPool(ISoundPool soundPool)
        {
            Assert.IsTrue(IsPooled);

            Reset();

            _soundPool = soundPool;
            _state = State.Fetched;
        }

        /// <summary>
        /// Callback for when this sound instance has just been returned to the pool. This should only ever be called by the SoundPool.
        /// </summary>
        public virtual void OnReturnedToPool()
        {
            Assert.IsFalse(IsPooled, "Tried to use SoundInstance while in pool. Make sure not to keep references to SoundInstances that have been destroyed.");

            Reset();

            _soundPool = null;
            _state = State.Pooled;
        }

        public virtual SoundInstance StopAndDestroy()
        {
            _soundPool.ReturnToPool(this);

            return null;
        }

        public abstract void Resume();
        public abstract void Pause();
        public abstract string GetStatusString();


    }
}
