using System;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// A serializable data structure representing a screen resulotion. 
    /// </summary>
    [Serializable]
    public struct ResolutionOption : IEquatable<Resolution>, IEquatable<ResolutionOption>, IComparable<ResolutionOption>
    {
        public int Width => _width;
        public int Height => _height;
        public int RefreshRate => _refreshRate;
        public Vector2Int Dimensions => new Vector2Int(_width, _height);

        [SerializeField]
        private int _width;

        [SerializeField]
        private int _height;

        [SerializeField]
        private int _refreshRate;

        private static Resolution[] _resoltions;

        public ResolutionOption(Resolution resolution)
        {
            _width = resolution.width;
            _height = resolution.height;
            _refreshRate = resolution.refreshRate;
        }

        public bool IsSupported()
        {
            if (_width <= 0) return false;
            if (_height <= 0) return false;
            if (_refreshRate <= 0) return false;

            if (_resoltions == null)
            {
                _resoltions = Screen.resolutions;
            }

            for (int i = 0; i < _resoltions.Length; i++)
            {
                if (_width == _resoltions[i].width && _height == _resoltions[i].height && _refreshRate == _resoltions[i].refreshRate)
                {
                    return true;
                }
            }

            return false;
        }

        public void Apply(FullScreenMode fullscreenMode)
        {
#if !UNITY_EDITOR
        UnityEngine.Assertions.Assert.IsTrue(IsSupported());
#endif
            Screen.SetResolution(_width, _height, fullscreenMode, _refreshRate);
            Canvas.ForceUpdateCanvases();
        }

        public static ResolutionOption GetFromString(string value)
        {
            int xIndex = value.IndexOf(" x ");
            int bracketIndex = value.IndexOf(" (");

            int width = int.Parse(value.Substring(0, xIndex));
            int height = int.Parse(value.Substring(xIndex + 3, bracketIndex - xIndex - 3));
            int refreshRate = int.Parse(value.Substring(bracketIndex + 2, value.Length - bracketIndex - 5));

            return new ResolutionOption()
            {
                _height = height,
                _width = width,
                _refreshRate = refreshRate
            };
        }

        public static ResolutionOption GetLargestSupportedResolution()
        {
            ResolutionOption maxResolution = new ResolutionOption();

            if (_resoltions == null)
            {
                _resoltions = Screen.resolutions;
            }

            for (int i = 0; i < _resoltions.Length; i++)
            {
                ResolutionOption resolution = new ResolutionOption(_resoltions[i]);
                if (resolution > maxResolution)
                {
                    maxResolution = resolution;
                }
            }

            if (maxResolution.IsSupported())
            {
                return maxResolution;
            }

            return new ResolutionOption(Screen.currentResolution);
        }

        public static ResolutionOption GetCurrentResolution()
        {
            return new ResolutionOption(Screen.currentResolution);
        }

        public static int CountSupportedResolutions()
        {
            return Screen.resolutions.Length;
        }

        public static ResolutionOption[] GetSupportedResolutions()
        {
            SortedList<ResolutionOption> supportedResolutions = new SortedList<ResolutionOption>();

            if (_resoltions == null)
            {
                _resoltions = Screen.resolutions;
            }

            for (int i = 0; i < _resoltions.Length; i++)
            {
                supportedResolutions.Add(new ResolutionOption(_resoltions[i]));
            }

            return supportedResolutions.ToArray();
        }

        public bool Equals(Resolution other)
        {
            return _width == other.width && _height == other.height && _refreshRate == other.refreshRate;
        }

        public override bool Equals(object obj)
        {
            if (obj is ResolutionOption)
            {
                return Equals((ResolutionOption)obj);
            }
            return false;
        }

        public bool Equals(ResolutionOption other)
        {
            return _width == other._width && _height == other._height && _refreshRate == other._refreshRate;
        }

        public int CompareTo(ResolutionOption other)
        {
            if (_width * _height > other._width * other._height) return 1;
            if (_width * _height == other._width * other._height)
            {
                if (_width > other._width) return 1;

                return _refreshRate.CompareTo(other._refreshRate);
            }
            return -1;
        }

        public override string ToString()
        {
            return _width + " x " + _height + " (" + _refreshRate + "Hz)";
        }

        public static bool operator ==(ResolutionOption x, ResolutionOption y)
        {
            return x._width == y._width && x._height == y._height && x._refreshRate == y._refreshRate;
        }

        public static bool operator !=(ResolutionOption x, ResolutionOption y)
        {
            return x._width != y._width || x._height != y._height || x._refreshRate != y._refreshRate;
        }

        public static bool operator >(ResolutionOption x, ResolutionOption y)
        {
            return x.CompareTo(y) > 0;
        }

        public static bool operator <(ResolutionOption x, ResolutionOption y)
        {
            return x.CompareTo(y) < 0;
        }

        public static bool operator >=(ResolutionOption x, ResolutionOption y)
        {
            return x.CompareTo(y) >= 0;
        }

        public static bool operator <=(ResolutionOption x, ResolutionOption y)
        {
            return x.CompareTo(y) <= 0;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _width;
                hashCode = (hashCode * 397) ^ _height;
                hashCode = (hashCode * 397) ^ _refreshRate;
                return hashCode;
            }
        }


    }
}