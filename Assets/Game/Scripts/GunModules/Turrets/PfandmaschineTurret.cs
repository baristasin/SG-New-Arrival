using FMODUnity;
using UnityEngine;

namespace Game.Scripts.GunModules.Turrets
{

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
