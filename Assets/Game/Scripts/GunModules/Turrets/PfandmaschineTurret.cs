using UnityEngine;

namespace Game.Scripts.GunModules.Turrets
{
    public class PfandmaschineTurret : ProjectileTurretBase
    {
        [SerializeField] private AudioClip _fireClip;
        [SerializeField, Range(0f, 2f)] private float _fireVolume = 0.7f;

        protected override bool TryFire()
        {
            var p = LaunchAtNearest();
            if (p == null) return false;

            if (_fireClip != null)
                AudioSource.PlayClipAtPoint(_fireClip, transform.position, _fireVolume);

            return true;
        }
    }
}
