using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class SeedList : IEnumerable<float>
    {
        public int Count
        {
            get => _seeds.Count;
            set => SetCount(value);
        }
        public bool IsReadOnly => false;

        public float this[int index]
        {
            get { EnsureCount(index + 1); return _seeds[index]; }
            set { EnsureCount(index + 1); _seeds[index] = value; }
        }

        private List<float> _seeds = new List<float>();
        private float _seedRange;

        public SeedList(float seedRange = 1f)
        {
            _seedRange = seedRange;
        }

        public IEnumerator<float> GetEnumerator()
        {
            return _seeds.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public float GetValue(int index)
        {
            EnsureCount(index + 1);

            return _seeds[index];
        }

        public void Randomize()
        {
            for (int i = 0; i < _seeds.Count; i++)
            {
                _seeds[i] = Random.value * _seedRange;
            }
        }

        public void Clear()
        {
            _seeds.Clear();
        }

        void EnsureCount(int count)
        {
            if (_seeds.Count < count)
            {
                while (_seeds.Count < count)
                {
                    _seeds.Add(Random.value * _seedRange);
                }
            }
        }

        void SetCount(int count)
        {
            if (_seeds.Count > count)
            {
                _seeds.RemoveRange(count, _seeds.Count - count);
            }
            else
            {
                while (_seeds.Count < count)
                {
                    _seeds.Add(Random.value * _seedRange);
                }
            }
        }

        public void CopyTo(float[] array, int arrayIndex)
        {
            _seeds.CopyTo(array, arrayIndex);
        }

    }
}
