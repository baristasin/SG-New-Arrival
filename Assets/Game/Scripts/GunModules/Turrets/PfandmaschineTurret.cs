using FMODUnity;
using UnityEngine;

namespace Game.Scripts.GunModules.Turrets
{
    // Pfandmaschine: fires homing bottles on a fixed (fast) cadence (WeaponData.FireInterval),
    // with a small splash on impact (set _areaRadius > 0 in the inspector).
    public class PfandmaschineTurret : ProjectileTurretBase
    {
        [SerializeField] private EventReference _fireEvent;

        protected override bool TryFire()
        {
            var p = LaunchAtNearest();
            if (p != null && !_fireEvent.IsNull)
                AudioManager.Instance.PlayOneShot(_fireEvent, transform.position);
            return p != null;
        }
    }
}
