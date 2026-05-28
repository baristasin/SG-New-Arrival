using System.Collections.Generic;
using Game.Scripts.ZombieModules;
using UnityEngine;

namespace Game.Scripts.GunModules
{
    // Kuckucksuhr: while held, punches a spring-mounted cuckoo straight out to WeaponData.Range
    // over _extendTime, then retracts over _retractTime (the two together = the ~2s cadence).
    // Zombies the cuckoo sweeps over on the way out take light damage + a big, chaotic knockback.
    public class CuckooClockWeapon : WeaponBase
    {
        [SerializeField] private Transform _muzzle;     // origin + aim (forward)
        [SerializeField] private Transform _cuckoo;     // the bird/spring visual that extends
        [SerializeField] private LayerMask _targetLayerMask;
        [SerializeField] private AudioSource _cuckooSound;

        [Header("Spring punch")]
        [SerializeField] private float _extendTime = 1f;
        [SerializeField] private float _retractTime = 1f;
        [SerializeField] private float _hitRadius = 0.8f;
        [SerializeField] private AnimationCurve _springCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Chaotic knockback")]
        [SerializeField] private float _knockChaosAngle = 25f;   // +/- degrees of random spread
        [SerializeField] private float _knockChaosAmount = 0.3f; // +/- fraction of distance

        private bool _isPunching;
        private float _punchTime;
        private Vector3 _punchDir;
        private readonly Collider[] _buffer = new Collider[16];
        private readonly HashSet<Collider> _hitThisPunch = new();

        private void OnEnable()
        {
            _isPunching = false;
            if (_cuckoo != null && _muzzle != null)
                _cuckoo.position = _muzzle.position;
        }

        public override void Tick(bool aimHeld, bool fireHeld)
        {
            if (_isPunching)
            {
                AdvancePunch();
                return;
            }

            if (aimHeld && fireHeld)
                StartPunch();
        }

        private void StartPunch()
        {
            _isPunching = true;
            _punchTime = 0f;
            _punchDir = _muzzle.forward;
            _hitThisPunch.Clear();
            if (_cuckooSound != null) _cuckooSound.Play();
        }

        private void AdvancePunch()
        {
            _punchTime += Time.deltaTime;
            bool extending = _punchTime < _extendTime;

            float frac = extending
                ? _springCurve.Evaluate(Mathf.Clamp01(_punchTime / _extendTime))
                : _springCurve.Evaluate(Mathf.Clamp01(1f - (_punchTime - _extendTime) / _retractTime));

            Vector3 origin = _muzzle.position;
            _cuckoo.position = origin + _punchDir * (frac * _data.Range);
            _cuckoo.rotation = Quaternion.LookRotation(_punchDir);

            if (extending)
                HitAtCuckoo();

            if (_punchTime >= _extendTime + _retractTime)
            {
                _isPunching = false;
                _cuckoo.position = origin;
            }
        }

        private void HitAtCuckoo()
        {
            int count = Physics.OverlapSphereNonAlloc(_cuckoo.position, _hitRadius, _buffer, _targetLayerMask);
            for (int i = 0; i < count; i++)
            {
                var col = _buffer[i];
                if (!_hitThisPunch.Add(col)) continue;   // each zombie only once per punch
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
