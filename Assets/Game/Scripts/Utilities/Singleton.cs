using UnityEngine;

namespace Game.Scripts.Utilities
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {        
        private static T _instance;
        private static readonly object _lock = new object();

        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = FindFirstObjectByType<T>();
                        if (_instance == null)
                        {
                            GameObject singletonObject = new GameObject(typeof(T).Name);
                            _instance = singletonObject.AddComponent<T>();
                        }
                    }
                    return _instance;
                }
            }
        }
    }
}