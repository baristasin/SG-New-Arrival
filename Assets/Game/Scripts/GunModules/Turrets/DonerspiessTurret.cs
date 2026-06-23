using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using Game.Scripts.ZombieModules;
using UnityEngine;

namespace Game.Scripts.GunModules.Turrets
{
    // Dönerspieß: a continuously rotating skewer that damages the zombies its blade sweeps over
    // (capsule along the skewer + per-zombie hit cooldown), like a hands-free melee weapon.
    public class DonerspiessTurret : TurretBase
    {
        [SerializeField] private Transform _skewer;
        [SerializeField] private float _rotationSpeed = 180f;

        [Header("Blade hit (capsule along the skewer)")]
        [SerializeField] private float _skewerLength = 2.5f;
        [SerializeField] private float _skewerRadius = 0.4f;
        [SerializeField] private float _hitCooldown = 0.5f;

        [Header("Audio")]
        [Tooltip("Looping spin sound while the turret is enabled. Leave null to disable.")]
        [SerializeField] private EventReference _spinLoopEvent;

        private readonly Collider[] _hitBuffer = new Collider[16];
        private readonly Dictionary<Collider, float> _hitTimestamps = new();
        private EventInstance _spinInstance;

        private void OnEnable()
        {
            if (!_spinLoopEvent.IsNull && !_spinInstance.isValid())
                _spinInstance = AudioManager.Instance.PlayLoopAttached(_spinLoopEvent, gameObject);
        }

        private void OnDisable()
        {
            if (_spinInstance.isValid()) AudioManager.Instance.Stop(ref _spinInstance);
        }

        protected override void OnUpdate()
        {
            if (_skewer == null) return;

            _skewer.Rotate(0f, _rotationSpeed * Time.deltaTime, 0f, Space.Self);
            SweepDamage();
        }

        // Capsule from the skewer base out along its forward axis = the blade. As the skewer
        // turns, the capsule sweeps a circle; only zombies it currently overlaps take damage.
        private void SweepDamage()
        {
            Vector3 start = _skewer.position;
            Vector3 end = start + _skewer.forward * _skewerLength;

            int count = Physics.OverlapCapsuleNonAlloc(start, end, _skewerRadius, _hitBuffer, _targetLayerMask);
            for (int i = 0; i < count; i++)
            {
                var col = _hitBuffer[i];

                if (_hitTimestamps.TryGetValue(col, out float last) && Time.time - last < _hitCooldown)
                    continue;

                if (ZombieRegistry.TryGetHealth(col, out var health))
                {
                    health.GetHit(_data.Damage);
                    _hitTimestamps[col] = Time.time;
                }
            }
        }
    }
}
