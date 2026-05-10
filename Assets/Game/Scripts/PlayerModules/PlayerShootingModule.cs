using Game.Scripts.GunModules.ProjectileGuns;
using UnityEngine;

namespace Game.Scripts.PlayerModules
{
    public class PlayerShootingModule : MonoBehaviour
    {
        [SerializeField] private GameObject[] _weapons;
        [SerializeField] private ProjectileGunData[] _weaponData;

        private int _currentIndex = -1;
        private ProjectileGunBase _currentGun;

        private void Awake()
        {
            EquipWeapon(0);
        }

        private void Update()
        {
            for (int i = 0; i < _weapons.Length; i++)
            {
                if (Input.GetKeyDown((i + 1).ToString()))
                    EquipWeapon(i);
            }

            if (_currentGun == null) return;

            if (Input.GetMouseButton(1))
            {
                _currentGun.ShowAimIndicator();

                if (Input.GetMouseButton(0))
                    _currentGun.TryShoot();
            }
            else
            {
                _currentGun.HideAimIndicator();
            }
        }

        private void EquipWeapon(int index)
        {
            if (_currentIndex == index) return;

            for (int i = 0; i < _weapons.Length; i++)
                _weapons[i].SetActive(i == index);

            _currentIndex = index;
            _currentGun = _weapons[index].GetComponent<ProjectileGunBase>();

            if (_currentGun != null && _weaponData.Length > index)
                _currentGun.InitializeGun(_weaponData[index]);
        }
    }
}
