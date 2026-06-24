using System.Collections.Generic;
using FMODUnity;
using Game.Scripts.ZombieModules;
using UnityEngine;

namespace Game.Scripts.GunModules
{
    // Bretzel boomerang. Hierarchy: Player → Bretzel (this script) → Model. transform.forward
    // is the throw direction; transform.position is the orbit centre and is read every frame
    // so the arc tracks the player. Model has no collider; damage is dealt by OverlapSphere
    // at the model's position.
    public class BretzelWeapon : WeaponBase
    {
        [SerializeField] private Transform _model;            // the thrown child
        [SerializeField] private LayerMask _targetLayerMask;  // zombie layer
        [SerializeField] private EventReference _throwEvent;
        [SerializeField, Range(0f, 2f)] private float _throwVolume = 0.7f;

        [SerializeField] private float _flightTime = 1.5f;
        [SerializeField] private float _arcWidth = 2.5f;
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
            SnapHome();
        }

        public override void Tick(bool aimHeld, bool fireHeld)
        {
            if (_cooldown > 0f) _cooldown -= Time.deltaTime;
            if (_isThrowing) { Advance(); return; }
            if (aimHeld && fireHeld && _cooldown <= 0f) StartThrow();
        }

        private void StartThrow()
        {
            _isThrowing = true;
            _flightProgress = 0f;
            _cooldown = _data.FireInterval;

            Vector3 fwd = transform.forward;
            fwd.y = 0f;
            _throwDir = fwd.sqrMagnitude > 0.0001f ? fwd.normalized : Vector3.forward;
            _throwRight = Vector3.Cross(Vector3.up, _throwDir);

            _hitThisThrow.Clear();
            _clearedAtMidpoint = false;

            _model.SetParent(null, true);   // unparent so world-position writes are clean
            NotifyFired();
            PlayShotSfx(_throwEvent, _throwVolume);
        }

        private void Advance()
        {
            _flightProgress += Time.deltaTime;
            float u = Mathf.Clamp01(_flightProgress / _flightTime);

            float forward = Mathf.Sin(u * Mathf.PI) * _data.Range;        // 0 → range → 0
            float lateral = Mathf.Sin(u * 2f * Mathf.PI) * _arcWidth;     // right out, left back

            // Orbit centre = our position THIS frame → arc moves with the player.
            _model.position = transform.position + _throwDir * forward + _throwRight * lateral;
            _model.Rotate(0f, _spinSpeed * Time.deltaTime, 0f, Space.Self);

            if (u >= 0.5f && !_clearedAtMidpoint) { _hitThisThrow.Clear(); _clearedAtMidpoint = true; }
            HitAtModel();

            if (_flightProgress >= _flightTime) { _isThrowing = false; SnapHome(); }
        }

        private void SnapHome()
        {
            if (_model == null) return;
            _model.SetParent(transform, false);
            _model.localPosition = Vector3.zero;
            _model.localRotation = Quaternion.identity;
        }

        private void HitAtModel()
        {
            int n = Physics.OverlapSphereNonAlloc(_model.position, _hitRadius, _buffer, _targetLayerMask);
            for (int i = 0; i < n; i++)
            {
                var col = _buffer[i];
                if (!_hitThisThrow.Add(col)) continue;
                if (ZombieRegistry.TryGetHealth(col, out var health))
                    health.GetHit(_data.Damage);
            }
        }
    }
}
