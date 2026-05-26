using Game.Scripts.ZombieModules;
using UnityEngine;

namespace Game.Scripts.GunModules.Turrets
{
    // Autonomous placed weapon: ticks itself, finds zombies in WeaponData.Range, fires on a
    // cadence. Shares WeaponData with player weapons but is not input-driven (no WeaponBase).
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

        // Per-frame hook (e.g. rotating a visual). Cadence-independent.
        protected virtual void OnUpdate() { }

        // Cadence fire used by projectile turrets; returns true if it actually fired so the
        // cooldown only resets on a real shot. Continuous turrets (e.g. Dönerspieß) leave this
        // as the default no-op and do their work per-frame in OnUpdate instead.
        protected virtual bool TryFire() => false;

        // Colliders within Range on the target layer, written into a shared buffer.
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
