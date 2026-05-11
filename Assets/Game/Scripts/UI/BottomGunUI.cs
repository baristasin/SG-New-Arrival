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
    }
}