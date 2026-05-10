using Game.Scripts.Utilities;
using UnityEngine;

namespace Game.Scripts.ZombieModules
{
    public class ZombieHealthBarPool : MonoBehaviour
    {
        [SerializeField] private ZombieHealthBar _prefab;
        [SerializeField] private int _initialSize = 10;

        private static Pool<ZombieHealthBar> _pool;

        private void Awake()
        {
            _pool = new Pool<ZombieHealthBar>(_prefab, _initialSize);
        }

        public static void Show(Transform target, float healthPercentage)
        {
            var bar = _pool.Get();
            bar.Show(target, healthPercentage);
        }

        public static void Return(ZombieHealthBar bar)
        {
            _pool.ReturnToPool(bar);
        }
    }
}
