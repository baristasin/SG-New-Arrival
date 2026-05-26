using Game.Scripts.GunModules.Projectiles;

namespace Game.Scripts.GunModules.Turrets
{
    // Copy machine: launches a homing paper plane, then waits until it lands before launching the
    // next — only one plane in flight at a time. Long range, single target (set FireInterval ~0).
    public class CopyMachineTurret : ProjectileTurretBase
    {
        private Projectile _currentPlane;

        protected override bool TryFire()
        {
            // Still in flight? hold the next plane.
            if (_currentPlane != null && _currentPlane.gameObject.activeInHierarchy)
                return false;

            _currentPlane = LaunchAtNearest();
            return _currentPlane != null;
        }
    }
}
