using Game.Scripts.GunModules.ProjectileGuns;
using UnityEngine;

namespace Game.Scripts.PlayerModules
{
    public class PlayerShootingModule : MonoBehaviour
    {
        [SerializeField] private ProjectileGunBase _currentProjectileGun;

        private void Awake()
        {
            ProjectileGunData pgd = new ProjectileGunData();
            pgd.FireRate = 0.5f;
            pgd.Damage = 25;
            pgd.ProjectileGunType = ProjectileGunType.Staple;
            pgd.Range = 50f;

            _currentProjectileGun.InitializeGun(pgd);
        }

        private void Update()
        {
            if (Input.GetKeyDown("1"))
            {
                ProjectileGunData pgd = new ProjectileGunData();
                pgd.FireRate = 0.5f;
                pgd.Damage = 25;
                pgd.ProjectileGunType = ProjectileGunType.Staple;
                pgd.Range = 50f;

                _currentProjectileGun.InitializeGun(pgd);
            }
            if (Input.GetKeyDown("2"))
            {
                ProjectileGunData pgd = new ProjectileGunData();
                pgd.FireRate = 0.08f;
                pgd.Damage = 25;
                pgd.ProjectileGunType = ProjectileGunType.Staple;
                pgd.Range = 50f;

                _currentProjectileGun.InitializeGun(pgd);
            }

            if (Input.GetMouseButton(1))
            {
                _currentProjectileGun.ShowAimIndicator();

                if (Input.GetMouseButton(0))
                    _currentProjectileGun.TryShoot();
            }
            else
            {
                _currentProjectileGun.HideAimIndicator();
            }
        }
    }
}
