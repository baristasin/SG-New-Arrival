using System;
using UnityEngine;

namespace Game.Scripts.GunModules
{
    public abstract class WeaponBase : MonoBehaviour
    {
        [SerializeField] protected WeaponData _data;

        public WeaponData Data => _data;
        
        public static event Action<NoiseLevel> OnFired;

        protected void NotifyFired()
        {
            if (_data != null) OnFired?.Invoke(_data.Noise);
        }
        
        protected void PlayShotSfx(FMODUnity.EventReference fmodEvent, float volume)
        {
            if (fmodEvent.IsNull || AudioManager.Instance == null) return;
            AudioManager.Instance.PlayOneShotWithVolume(fmodEvent, transform.position, volume);
        }
        
        public abstract void Tick(bool aimHeld, bool fireHeld);

        public virtual void ShowAimIndicator() { }
        public virtual void HideAimIndicator() { }
    }
}
