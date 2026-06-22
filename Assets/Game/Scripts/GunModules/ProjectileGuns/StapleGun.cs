using Game.Scripts.ZombieModules;
using UnityEngine;

namespace Game.Scripts.GunModules.ProjectileGuns
{
    public class StapleGun : RangedWeaponBase
    {
        [SerializeField] private LineRenderer _lineRenderer;

        protected override void Fire()
        {
            if (!Physics.Raycast(_muzzlePoint.position, _muzzlePoint.forward, out var hit, _data.Range, _targetLayerMask))
                return;

            if (ZombieRegistry.TryGetHealth(hit.collider, out var health))
            {
                Vector3 knockDir = _muzzlePoint.forward.normalized;
                knockDir.y = 0f;
                health.GetHit(_data.Damage, knockDir);
            }
        }

        public override void ShowAimIndicator()
        {
            _lineRenderer.enabled = true;
            _lineRenderer.SetPosition(0, _muzzlePoint.position);

            Vector3 endPoint = _muzzlePoint.position + _muzzlePoint.forward * _data.Range;
            if (Physics.Raycast(_muzzlePoint.position, _muzzlePoint.forward, out var hit, _data.Range, _targetLayerMask))
                endPoint = hit.point;

            _lineRenderer.SetPosition(1, endPoint);
        }

        public override void HideAimIndicator()
        {
            _lineRenderer.enabled = false;
        }
    }
}
