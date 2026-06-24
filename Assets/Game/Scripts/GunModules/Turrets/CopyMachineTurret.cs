using FMODUnity;
using Game.Scripts.GunModules.Projectiles;
using UnityEngine;

namespace Game.Scripts.GunModules.Turrets
{

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
