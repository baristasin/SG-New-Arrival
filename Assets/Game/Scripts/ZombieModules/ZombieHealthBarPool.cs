using System.Collections.Generic;
using Game.Scripts.Utilities;
using UnityEngine;

namespace Game.Scripts.ZombieModules
{
    // Pool for the floating zombie health bars. Tracks which bar belongs to which target so each
    // zombie only ever has ONE bar — repeated hits update the same bar instead of stacking new
    // ones that linger on the dead zombie during its death animation.
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

        // Reuses the existing bar if one is already tracking this target; otherwise pulls a fresh
        // one from the pool.
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

        // Called by ZombieController.ZombieDead — instantly removes the bar in the same frame
        // the zombie dies, instead of waiting for LateUpdate / display-duration.
        public static void HideFor(Transform target)
        {
            if (target == null) return;
            if (_activeBars.TryGetValue(target, out var bar) && bar != null)
                bar.HideImmediate();
        }

        public static void Return(ZombieHealthBar bar)
        {
            if (bar == null) return;

            // Clear the target → bar mapping using the bar's still-set target.
            if (bar.Target != null) _activeBars.Remove(bar.Target);

            _pool.ReturnToPool(bar);
        }
    }
}
