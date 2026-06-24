using System.Collections.Generic;
using Game.Scripts.Utilities;
using UnityEngine;

namespace Game.Scripts.ZombieModules
{

    public class ZombieHealthBarPool : MonoBehaviour
    {
        [SerializeField] private ZombieHealthBar _prefab;
        [SerializeField] private int _initialSize = 10;

        private static Pool<ZombieHealthBar> _pool;
        private static readonly Dictionary<Transform, ZombieHealthBar> _activeBars = new();

        private void Awake()
        {
            _pool = new Pool<ZombieHealthBar>(_prefab, _initialSize);
            _activeBars.Clear();
        }


        public static void Show(Transform target, int remainingHealth, int maxHealth)
        {
            if (target == null) return;

            if (!_activeBars.TryGetValue(target, out var bar) || bar == null)
            {
                bar = _pool.Get();
                _activeBars[target] = bar;
            }
            bar.Show(target, remainingHealth, maxHealth);
        }

  
        public static void HideFor(Transform target)
        {
            if (target == null) return;
            if (_activeBars.TryGetValue(target, out var bar) && bar != null)
                bar.HideImmediate();
        }

        public static void Return(ZombieHealthBar bar)
        {
            if (bar == null) return;

            if (bar.Target != null) _activeBars.Remove(bar.Target);

            _pool.ReturnToPool(bar);
        }
    }
}
