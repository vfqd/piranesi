using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    [Serializable]
    public class ScriptableEnumMask<T> : ScriptableEnumMask where T : ScriptableEnum
    {
        public override bool IsNothing => !_isEverything && _values.Count == 0;
        public override int Count => _isEverything ? ScriptableEnum.GetCount<T>() : _values.Count;
        public T this[int index] => _isEverything ? ScriptableEnum.GetValue<T>(index) : _values[index];

        [SerializeField]
        private List<T> _values = new List<T>();

        public void Add(T value)
        {
            if (!_values.Contains(value))
            {
                _values.Add(value);
            }
        }

        public void Remove(T value)
        {
            _values.Remove(value);
        }

        public bool Contains(T value)
        {
            if (_isEverything) return true;

            for (int i = 0; i < _values.Count; i++)
            {
                if (_values[i] == value) return true;
            }

            return false;
        }

        public void SetAsEverything()
        {
            _values = new List<T>();
            _isEverything = true;
        }



        public override int GetBitMask()
        {

            int mask = 0;
            T[] allValues = ScriptableEnum.GetValues<T>();

            for (int i = 0; i < allValues.Length; i++)
            {
                if (_isEverything || _values.Contains(allValues[i]))
                {
                    mask |= (1 << i);
                }
            }

            return mask;
        }

        public override void SetFromBitMask(int mask)
        {
            _values = new List<T>();
            T[] allValues = ScriptableEnum.GetValues<T>();

            for (int i = 0; i < allValues.Length; i++)
            {
                if (((1 << i) & mask) != 0)
                {
                    _values.Add(allValues[i]);
                }
            }

            if (_values.Count == allValues.Length)
            {
                _isEverything = true;
                _values.Clear();
            }
            else
            {
                _isEverything = false;
            }

        }


        public override ScriptableEnum[] GetRawValues()
        {
            return _isEverything ? ScriptableEnum.GetValues<T>() : _values.ToArray();
        }

    }

    [Serializable]
    public abstract class ScriptableEnumMask
    {
        public bool IsEverything => _isEverything;
        public abstract bool IsNothing { get; }
        public abstract int Count { get; }

        [SerializeField]
        protected bool _isEverything;

        public abstract int GetBitMask();
        public abstract void SetFromBitMask(int mask);
        public abstract ScriptableEnum[] GetRawValues();


    }
}