using System;
using UnityEngine;

namespace Game.Scripts.GunModules
{
    public abstract class WeaponBase : MonoBehaviour
    {
        [SerializeField] protected WeaponData _data;

        public WeaponData Data => _data;

        // Fired every time a weapon discharges (shot / throw / swing). The night gate listens to
        // this to penalise Loud shots during Ruhezeit. Subclasses call NotifyFired() in their
        // attack methods.
        public static event Action<NoiseLevel> OnFired;

        protected void NotifyFired()
        {
            if (_data != null) OnFired?.Invoke(_data.Noise);
        }

        // Plays the weapon's shot SFX through FMOD at the weapon's current world position.
        // FMOD attenuates relative to the listener, so for the volume to stay flat the FMOD
        // Studio Listener must be attached to (or follow) the player.
        protected void PlayShotSfx(FMODUnity.EventReference fmodEvent, float volume)
        {
            if (fmodEvent.IsNull || AudioManager.Instance == null) return;
            AudioManager.Instance.PlayOneShotWithVolume(fmodEvent, transform.position, volume);
        }

        // Driven each frame by PlayerShootingModule while this weapon is equipped.
        // aimHeld is the legacy aim input (always true now); fireHeld is LMB.
        public abstract void Tick(bool aimHeld, bool fireHeld);

        public virtual void ShowAimIndicator() { }
        public virtual void HideAimIndicator() { }
    }
}
