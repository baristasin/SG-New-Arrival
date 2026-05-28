using System.Collections.Generic;
using Game.Scripts.ZombieModules;
using UnityEngine;

namespace Game.Scripts.GunModules
{
    // Bretzel: while held, throws a spinning pretzel on a slow cadence (WeaponData.FireInterval).
    // The pretzel flies a boomerang loop out to WeaponData.Range and back to the (moving) player
    // over _flightTime, hitting every zombie along its path once per throw. Medium damage, no knockback.
    public class BretzelWeapon : WeaponBase
    {
        [SerializeField] private Transform _muzzle;     // origin + aim (forward)
        [SerializeField] private Transform _pretzel;    // the thrown visual (hidden at rest)
        [SerializeField] private LayerMask _targetLayerMask;

        [Header("Boomerang flight")]
        [SerializeField] private float _flightTime = 1.5f;
        [SerializeField] private float _arcWidth = 2.5f;   // sideways curve amplitude
        [SerializeField] private float _spinSpeed = 720f;
        [SerializeField] private float _hitRadius = 0.7f;

        private bool _isThrowing;
        private float _flightProgress;
        private float _cooldown;
        private Vector3 _throwDir;
        private Vector3 _throwRight;
        private bool _clearedAtMidpoint;
        private readonly Collider[] _buffer = new Collider[16];
        private readonly HashSet<Collider> _hitThisThrow = new();

        private void OnEnable()
        {
            _isThrowing = false;
            _cooldown = 0f;
            if (_pretzel != null) _pretzel.gameObject.SetActive(false);
        }

        public override void Tick(bool aimHeld, bool fireHeld)
        {
            if (_cooldown > 0f) _cooldown -= Time.deltaTime;

            if (_isThrowing)
            {
                AdvanceThrow();
                return;
            }

            if (aimHeld && fireHeld && _cooldown <= 0f)
                StartThrow();
        }

        private void StartThrow()
        {
            _isThrowing = true;
            _flightProgress = 0f;
            _cooldown = _data.FireInterval;

            _throwDir = _muzzle.forward;
            _throwDir.y = 0f;
            _throwDir.Normalize();
            _throwRight = _muzzle.right;

            _hitThisThrow.Clear();
            _clearedAtMidpoint = false;
            if (_pretzel != null) _pretzel.gameObject.SetActive(true);
        }

        private void AdvanceThrow()
        {
            _flightProgress += Time.deltaTime;
            float u = Mathf.Clamp01(_flightProgress / _flightTime);

            float forward = Mathf.Sin(u * Mathf.PI) * _data.Range;          // 0 -> range -> 0
            float lateral = Mathf.Sin(u * 2f * Mathf.PI) * _arcWidth;       // right on the way out, left back

            _pretzel.position = _muzzle.position + _throwDir * forward + _throwRight * lateral;
            _pretzel.Rotate(0f, _spinSpeed * Time.deltaTime, 0f, Space.Self);

            // At the farthest point, reset hits so the return pass can hit the same zombies again.
            if (u >= 0.5f && !_clearedAtMidpoint)
            {
                _hitThisThrow.Clear();
                _clearedAtMidpoint = true;
            }

            HitAlongPath();

            if (_flightProgress >= _flightTime)
            {
                _isThrowing = false;
                _pretzel.gameObject.SetActive(false);
            }
        }

        private void HitAlongPath()
        {
            int count = Physics.OverlapSphereNonAlloc(_pretzel.position, _hitRadius, _buffer, _targetLayerMask);
            for (int i = 0; i < count; i++)
            {
                var col = _buffer[i];
                if (!_hitThisThrow.Add(col)) continue;   // each zombie only once per throw
                if (ZombieRegistry.TryGetHealth(col, out var health))
                    health.GetHit(_data.Damage);
            }
        }
    }
}
