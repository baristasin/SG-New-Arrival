using FMODUnity;
using Game.Scripts.GunModules.Projectiles;
using UnityEngine;

namespace Game.Scripts.GunModules.Turrets
{
    // Copy machine: launches a homing paper plane, then waits until it lands before launching the
    // next — only one plane in flight at a time. Long range, single target (set FireInterval ~0).
    public class CopyMachineTurret : ProjectileTurretBase
    {
        [SerializeField] private EventReference _fireEvent;

        private Projectile _currentPlane;

        protected override bool TryFire()
        {
            // Still in flight? hold the next plane.
            if (_currentPlane != null && _currentPlane.gameObject.activeInHierarchy)
                return false;

            _currentPlane = LaunchAtNearest();
            if (_currentPlane != null && !_fireEvent.IsNull)
                AudioManager.Instance.PlayOneShot(_fireEvent, transform.position);
            return _currentPlane != null;
        }
    }
}
