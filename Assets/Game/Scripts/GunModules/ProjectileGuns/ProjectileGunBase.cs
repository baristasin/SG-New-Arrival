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

        protected ProjectileGunData _projectileGunData;

        private float _fireTimer;

        public virtual void InitializeGun(ProjectileGunData projectileGunData)
        {
            _projectileGunData = projectileGunData;
        }

        public void TryShoot()
        {
            if (_fireTimer > 0f) return;
            _fireTimer = _projectileGunData.FireRate;
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
