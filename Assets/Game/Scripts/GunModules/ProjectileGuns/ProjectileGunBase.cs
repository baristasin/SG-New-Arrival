using System;
using UnityEngine;

namespace Game.Scripts.GunModules.ProjectileGuns
{
    public enum ProjectileGunType
    {
        Staple,
    }

    [Serializable]
    public class ProjectileGunData
    {
        public ProjectileGunType ProjectileGunType;
        public int Damage;
        public float Range;
        public float FireRate;
    }

    public abstract class ProjectileGunBase : MonoBehaviour
    {
        [SerializeField] protected Transform _muzzlePoint;
        [SerializeField] protected LayerMask _targetLayerMask;
        [SerializeField] protected ParticleSystem _muzzleEffect;

        protected ProjectileGunData _projectileGunData;

        private float _fireTimer;

        [SerializeField] private EventReference _stapleGunEvent;
        
        public virtual void InitializeGun(ProjectileGunData projectileGunData)
        {
            _projectileGunData = projectileGunData;
        }

        public void TryShoot()
        {
            if (_fireTimer > 0f) return;
            AudioManager.Instance.PlayOneShotAttached(_stapleGunEvent, gameObject);
            _fireTimer = _projectileGunData.FireRate;
            if (_muzzleEffect != null) _muzzleEffect.Play();
            Fire();
        }

        protected abstract void Fire();

        public abstract void ShowAimIndicator();
        public abstract void HideAimIndicator();

        private void Update()
        {
            if (_fireTimer > 0f)
                _fireTimer -= Time.deltaTime;
        }
    }
}
