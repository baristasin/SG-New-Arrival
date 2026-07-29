using System;
using System.Collections.Generic;
using Game.Scripts.PlayerModules;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Night
{
    public class DayUnlocks : MonoBehaviour
    {
        [Serializable]
        public class DayUnlock
        {
            public int Day;
            public int WeaponsUnlocked = 1;
            public List<GameObject> Activate = new();
        }

        [SerializeField] private PlayerShootingModule _shootingModule;
        [SerializeField] private List<DayUnlock> _plan = new();

        private void Start()
        {
            int day = GameManager.Instance != null ? GameManager.Instance.NightDay : 1;
            ApplyForDay(day);
        }

        [Button]
        public void ApplyForDay(int day)
        {
            foreach (var entry in _plan)
                if (entry != null)
                    foreach (var obj in entry.Activate)
                        if (obj != null) obj.SetActive(false);

            int weapons = 0;
            foreach (var entry in _plan)
            {
                if (entry == null || entry.Day > day) continue;
                if (entry.WeaponsUnlocked > weapons) weapons = entry.WeaponsUnlocked;
                foreach (var obj in entry.Activate)
                    if (obj != null) obj.SetActive(true);
            }

            if (_shootingModule != null)
                _shootingModule.SetUnlockedCount(Mathf.Max(1, weapons));
        }
    }
}
