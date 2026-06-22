using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.UI
{
    public class BottomGunUI : MonoBehaviour
    {
        [SerializeField] private List<GunUIItem> _gunUIItems;

        private void Awake()
        {
            UnSelectAll();
            Select(0);
        }

        private void UnSelectAll()
        {
            for (int i = 0; i < _gunUIItems.Count; i++)
            {
                _gunUIItems[i].UnSelect();
            }
        }

        public void Select(int i)
        {
            UnSelectAll();
            _gunUIItems[i].Select();
        }

        // Hide icons for weapon slots that haven't been unlocked yet. Called by
        // PlayerShootingModule.SetUnlockedCount on day load.
        public void SetVisibleCount(int count)
        {
            for (int i = 0; i < _gunUIItems.Count; i++)
                if (_gunUIItems[i] != null)
                    _gunUIItems[i].gameObject.SetActive(i < count);
        }
    }
}