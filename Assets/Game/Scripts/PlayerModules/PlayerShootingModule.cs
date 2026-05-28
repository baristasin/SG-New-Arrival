using Game.Scripts.GunModules;
using Game.Scripts.UI;
using UnityEngine;

namespace Game.Scripts.PlayerModules
{
    public class PlayerShootingModule : MonoBehaviour
    {
        [SerializeField] private GameObject[] _weapons;
        [SerializeField] private BottomGunUI _bottomGunUI;

        private int _currentIndex = -1;
        private WeaponBase _current;

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

            if (_current == null) return;

            // aimHeld = RMB (the to-be-removed aim mechanic), fireHeld = LMB.
            _current.Tick(Input.GetMouseButton(1), Input.GetMouseButton(0));
        }

        private void EquipWeapon(int index)
        {
            if (_currentIndex == index) return;

            for (int i = 0; i < _weapons.Length; i++)
                _weapons[i].SetActive(i == index);

            _currentIndex = index;
            _current = _weapons[index].GetComponent<WeaponBase>();

            _bottomGunUI.Select(index);
        }
    }
}
