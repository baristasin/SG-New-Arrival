using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Utilities
{
    public class Pool<T> where T : Component
    {        
        private readonly T _prefab;
        private readonly Queue<T> _pool = new Queue<T>();

        public Pool(T prefab, int initialCount)
        {
            _prefab = prefab;
            for (int i = 0; i < initialCount; i++)
            {
                T obj = GameObject.Instantiate(_prefab);
                obj.gameObject.SetActive(false);
                _pool.Enqueue(obj);
            }
        }

        public T Get()
        {
            T obj = _pool.Count > 0 ? _pool.Dequeue() : GameObject.Instantiate(_prefab);
            obj.gameObject.SetActive(true);
            return obj;
        }

        public void ReturnToPool(T obj)
        {
            obj.gameObject.SetActive(false);
            _pool.Enqueue(obj);
        }
    }
}