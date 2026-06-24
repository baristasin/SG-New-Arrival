using System.Collections.Generic;
using Game.Scripts.ZombieModules;
using UnityEngine;

namespace Game.Scripts.GunModules
{
    public class MeleeWeaponModule : WeaponBase
    {
        [SerializeField] private float _slashThreshold = 10f;
        [SerializeField] private float _weaponLength = 2.5f;
        [SerializeField] private float _handleBackExtent = 1f;
        [SerializeField] private float _weaponRadius = 0.5f;
        [SerializeField] private float _hitCooldown = 0.3f;
        [SerializeField] private LayerMask _targetLayer;
        [SerializeField] private AudioClip _slashClip;
        [SerializeField, Range(0f, 2f)] private float _slashVolume = 0.7f;
        [SerializeField] private float _slashSoundCooldown = 0.3f;

        private float _lastAngle;
        private float _nextSlashSoundTime;
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
            {
                Slash(deltaAngle);
                NotifyFired();

                // Sound on every swing, but throttled so a fast continuous swing doesn't stack
                // a dozen plays into one frame's worth of audio.
                if (Time.time >= _nextSlashSoundTime && _slashClip != null && Camera.main != null)
                {
                    AudioSource.PlayClipAtPoint(_slashClip, Camera.main.transform.position, _slashVolume);
                    _nextSlashSoundTime = Time.time + _slashSoundCooldown;
                }
            }
        }

        private void Slash(float speed)
        {
            Vector3 start = transform.position - transform.forward * _handleBackExtent;
            Vector3 end = transform.position + transform.forward * _weaponLength;

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
