using Game.Scripts.GunModules;
using Game.Scripts.UI;
using UnityEngine;

namespace Game.Scripts.PlayerModules
{
    public class PlayerShootingModule : MonoBehaviour
    {
        [SerializeField] private GameObject[] _weapons;
        [SerializeField] private BottomGunUI _bottomGunUI;
        
        public int UnlockedCount { get; private set; } = -1;

        private int _currentIndex = -1;
        private WeaponBase _current;

        private void Awake()
        {
            EquipWeapon(0);
        }

        public void SetUnlockedCount(int count)
        {
            UnlockedCount = Mathf.Max(1, count);
            if (_bottomGunUI != null) _bottomGunUI.SetVisibleCount(UnlockedCount);
            
            int target = _currentIndex >= 0 && _currentIndex < UnlockedCount ? _currentIndex : 0;
            ForceEquip(target);
        }

        private void ForceEquip(int index)
        {
            for (int i = 0; i < _weapons.Length; i++)
                if (_weapons[i] != null) _weapons[i].SetActive(i == index);

            _currentIndex = index;
            _current = _weapons[index].GetComponent<WeaponBase>();

            if (_bottomGunUI != null) _bottomGunUI.Select(index);
        }

        private int MaxSelectable() =>
            UnlockedCount < 0 ? _weapons.Length : Mathf.Min(UnlockedCount, _weapons.Length);

        private void Update()
        {
            int max = MaxSelectable();
            for (int i = 0; i < max; i++)
            {
                if (Input.GetKeyDown((i + 1).ToString()))
                    EquipWeapon(i);
            }

            if (_current == null) return;
            
            _current.Tick(true, Input.GetMouseButton(0));
        }

        private void EquipWeapon(int index)
        {
            if (_currentIndex == index) return;
            if (index < 0 || index >= MaxSelectable()) return;   // locked

            for (int i = 0; i < _weapons.Length; i++)
                _weapons[i].SetActive(i == index);

            _currentIndex = index;
            _current = _weapons[index].GetComponent<WeaponBase>();

            if (_bottomGunUI != null) _bottomGunUI.Select(index);
        }
    }
}
