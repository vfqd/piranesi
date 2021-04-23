using System;
using UnityEngine;

////////// MONOSINGLETON ////////// 
namespace Utils
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static bool _isApplicationQuitting = false;
        
        protected MonoSingleton() { }

        public static T Instance
        {
            get
            {
                if (_instance == null)
                    _instance = (T)FindObjectOfType(typeof(T));

                if (_instance == null && !_isApplicationQuitting)
                {
                    Debug.LogError(typeof(T).ToString() + " cannot be found in the scene.");
                }

                return _instance;
            }
        }
        
        private void OnApplicationQuit () {
            _isApplicationQuitting = true;
        }
    }
}