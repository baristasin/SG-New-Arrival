using Game.Scripts.GunModules;
using Game.Scripts.UI;
using UnityEngine;

namespace Game.Scripts.PlayerModules
{
    public class PlayerShootingModule : MonoBehaviour
    {
        [SerializeField] private GameObject[] _weapons;
        [SerializeField] private BottomGunUI _bottomGunUI;

        // -1 = every weapon in _weapons is selectable. DayUnlocks sets this on Start to clamp
        // the available slots per day. EquipWeapon refuses anything beyond the count and the
        // keyboard input loop only listens for unlocked slots.
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

            // Force-reapply the active state. DayUnlocks runs AFTER Awake and may have toggled
            // some _weapons children on as part of its day plan; this guarantees that only the
            // currently equipped slot remains visible.
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

            // Always "aiming" — RMB gating is gone. fireHeld is LMB. Weapons that check both
            // (e.g. Bretzel, Cuckoo) fire whenever LMB is held.
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
