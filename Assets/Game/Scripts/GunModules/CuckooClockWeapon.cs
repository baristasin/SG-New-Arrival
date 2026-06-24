using System.Collections.Generic;
using FMODUnity;
using Game.Scripts.ZombieModules;
using UnityEngine;

namespace Game.Scripts.GunModules
{
    public class CuckooClockWeapon : WeaponBase
    {
        [SerializeField] private Transform _model;            // the cuckoo bird
        [SerializeField] private LayerMask _targetLayerMask;
        [SerializeField] private AudioSource _cuckooSound;
        [SerializeField] private EventReference _punchEvent;
        [SerializeField, Range(0f, 2f)] private float _punchVolume = 0.7f;

        [SerializeField] private float _extendTime = 1f;
        [SerializeField] private float _retractTime = 1f;
        [SerializeField] private float _hitRadius = 0.8f;
        [SerializeField] private AnimationCurve _springCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [SerializeField] private float _knockChaosAngle = 25f;
        [SerializeField] private float _knockChaosAmount = 0.3f;

        private bool _isPunching;
        private float _punchTime;
        private Vector3 _punchDir;
        private readonly Collider[] _buffer = new Collider[16];
        private readonly HashSet<Collider> _hitThisPunch = new();

        private void OnEnable()
        {
            _isPunching = false;
            SnapHome();
        }

        public override void Tick(bool aimHeld, bool fireHeld)
        {
            if (_isPunching) { Advance(); return; }
            if (aimHeld && fireHeld) StartPunch();
        }

        private void StartPunch()
        {
            _isPunching = true;
            _punchTime = 0f;
            _punchDir = transform.forward;
            _hitThisPunch.Clear();
            if (_cuckooSound != null) _cuckooSound.Play();

            _model.SetParent(null, true);
            NotifyFired();
            PlayShotSfx(_punchEvent, _punchVolume);
        }

        private void Advance()
        {
            _punchTime += Time.deltaTime;
            bool extending = _punchTime < _extendTime;

            float frac = extending
                ? _springCurve.Evaluate(Mathf.Clamp01(_punchTime / _extendTime))
                : _springCurve.Evaluate(Mathf.Clamp01(1f - (_punchTime - _extendTime) / _retractTime));

            // Spring origin = our position this frame → punch follows the player.
            _model.position = transform.position + _punchDir * (frac * _data.Range);
            _model.rotation = Quaternion.LookRotation(_punchDir);

            if (extending) HitAtModel();

            if (_punchTime >= _extendTime + _retractTime) { _isPunching = false; SnapHome(); }
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
                if (!_hitThisPunch.Add(col)) continue;
                if (!ZombieRegistry.TryGetHealth(col, out var health)) continue;

                Vector3 knockDir = Quaternion.Euler(0f, Random.Range(-_knockChaosAngle, _knockChaosAngle), 0f) * _punchDir;
                knockDir.y = 0f;
                knockDir.Normalize();

                float distance = _data.Knockback * Random.Range(1f - _knockChaosAmount, 1f + _knockChaosAmount);
                health.GetHit(_data.Damage, knockDir, distance);
            }
        }
    }
}
