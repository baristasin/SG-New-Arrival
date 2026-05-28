using UnityEngine;

namespace Game.Scripts.GunModules
{
    public abstract class WeaponBase : MonoBehaviour
    {
        [SerializeField] protected WeaponData _data;

        public WeaponData Data => _data;

        // Driven each frame by PlayerShootingModule while this weapon is equipped.
        // aimHeld is the current (to-be-removed) RMB aim input; fireHeld is LMB.
        public abstract void Tick(bool aimHeld, bool fireHeld);

        public virtual void ShowAimIndicator() { }
        public virtual void HideAimIndicator() { }
    }
}
