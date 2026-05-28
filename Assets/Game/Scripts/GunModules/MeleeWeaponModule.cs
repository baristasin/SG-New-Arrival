using System.Collections.Generic;
using Game.Scripts.ZombieModules;
using UnityEngine;

namespace Game.Scripts.GunModules
{
    // Swings while held, hitting enemies based on how fast the cursor (this transform) turns.
    public class MeleeWeaponModule : WeaponBase
    {
        [SerializeField] private float _slashThreshold = 10f;
        [SerializeField] private float _weaponLength = 2.5f;
        [SerializeField] private float _weaponRadius = 0.5f;
        [SerializeField] private float _hitCooldown = 0.3f;
        [SerializeField] private LayerMask _targetLayer;

        private float _lastAngle;
        private Collider[] _hitBuffer = new Collider[20];
        private Dictionary<Collider, float> _hitTimestamps = new();

        private void OnEnable()
        {
            _lastAngle = transform.eulerAngles.y;
        }

        public override void Tick(bool aimHeld, bool fireHeld)
        {
            float currentAngle = transform.eulerAngles.y;
            float deltaAngle = Mathf.Abs(Mathf.DeltaAngle(_lastAngle, currentAngle));
            _lastAngle = currentAngle;

            if (!aimHeld) return;

            if (deltaAngle > _slashThreshold)
                Slash(deltaAngle);
        }

        private void Slash(float speed)
        {
            Vector3 start = transform.position;
            Vector3 end = start + transform.forward * _weaponLength;

            int count = Physics.OverlapCapsuleNonAlloc(start, end, _weaponRadius, _hitBuffer, _targetLayer);

            int baseDamage = _data != null ? _data.Damage : 0;
            int damage = (int)(baseDamage * (speed / _slashThreshold));

            for (int i = 0; i < count; i++)
            {
                var col = _hitBuffer[i];

                if (_hitTimestamps.TryGetValue(col, out float lastHit) && Time.time - lastHit < _hitCooldown)
                    continue;

                if (ZombieRegistry.TryGetHealth(col, out var health))
                {
                    health.GetHit(damage);
                    _hitTimestamps[col] = Time.time;
                }
            }
        }
    }
}
