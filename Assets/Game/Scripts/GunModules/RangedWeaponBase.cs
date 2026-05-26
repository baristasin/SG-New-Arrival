using UnityEngine;

namespace Game.Scripts.GunModules
{
    // Fires Fire() on a fixed cadence while held, with optional magazine + reload.
    public abstract class RangedWeaponBase : WeaponBase
    {
        [SerializeField] protected Transform _muzzlePoint;
        [SerializeField] protected LayerMask _targetLayerMask;
        [SerializeField] protected ParticleSystem _muzzleEffect;

        public int AmmoInMagazine { get; private set; }
        public bool IsReloading { get; private set; }

        private float _cooldown;
        private float _reloadTimer;

        protected virtual void OnEnable()
        {
            AmmoInMagazine = _data != null ? _data.MagazineSize : 0;
            IsReloading = false;
            _cooldown = 0f;
        }

        public override void Tick(bool aimHeld, bool fireHeld)
        {
            if (_cooldown > 0f) _cooldown -= Time.deltaTime;

            if (IsReloading)
            {
                _reloadTimer -= Time.deltaTime;
                if (_reloadTimer <= 0f)
                {
                    IsReloading = false;
                    AmmoInMagazine = _data.MagazineSize;
                }
            }

            if (!aimHeld)
            {
                HideAimIndicator();
                return;
            }

            ShowAimIndicator();
            if (fireHeld) TryShoot();
        }

        private void TryShoot()
        {
            if (_data == null || IsReloading || _cooldown > 0f) return;

            bool usesMagazine = _data.MagazineSize > 0;
            if (usesMagazine && AmmoInMagazine <= 0)
            {
                BeginReload();
                return;
            }

            _cooldown = _data.FireInterval;
            if (usesMagazine) AmmoInMagazine--;

            if (_muzzleEffect != null) _muzzleEffect.Play();
            Fire();

            if (usesMagazine && AmmoInMagazine <= 0) BeginReload();
        }

        private void BeginReload()
        {
            if (IsReloading || _data == null || _data.MagazineSize <= 0) return;
            IsReloading = true;
            _reloadTimer = _data.ReloadTime;
        }

        protected abstract void Fire();
    }
}
