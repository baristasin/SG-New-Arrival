using Game.Scripts.ZombieModules;
using UnityEngine;

namespace Game.Scripts.GunModules.Turrets
{
    public abstract class TurretBase : MonoBehaviour
    {
        [SerializeField] protected WeaponData _data;
        [SerializeField] protected LayerMask _targetLayerMask;

        private readonly Collider[] _overlapBuffer = new Collider[32];
        private float _cooldown;

        public WeaponData Data => _data;

        private void Update()
        {
            OnUpdate();

            if (_cooldown > 0f)
            {
                _cooldown -= Time.deltaTime;
                return;
            }

            if (TryFire())
                _cooldown = _data != null ? _data.FireInterval : 0.5f;
        }

        protected virtual void OnUpdate() { }
        
        protected virtual bool TryFire() => false;

        protected int FindTargetsInRange(out Collider[] buffer)
        {
            buffer = _overlapBuffer;
            if (_data == null) return 0;
            return Physics.OverlapSphereNonAlloc(transform.position, _data.Range, _overlapBuffer, _targetLayerMask);
        }

        protected ZombieHealthModule GetNearestTarget()
        {
            int count = FindTargetsInRange(out var buffer);
            ZombieHealthModule nearest = null;
            float best = float.MaxValue;
            Vector3 pos = transform.position;

            for (int i = 0; i < count; i++)
            {
                if (!ZombieRegistry.TryGetHealth(buffer[i], out var health)) continue;
                float sqr = (buffer[i].transform.position - pos).sqrMagnitude;
                if (sqr < best)
                {
                    best = sqr;
                    nearest = health;
                }
            }

            return nearest;
        }
    }
}
