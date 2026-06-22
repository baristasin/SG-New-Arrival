using System;
using System.Collections.Generic;
using Game.Scripts.PlayerModules;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Night
{
    // Day-by-day progression of weapons and turrets in the NightCity scene. Each entry lists
    // what becomes available on a specific day — a cumulative weapon count, plus a set of
    // GameObjects to activate (turrets, props, extras). On Start the script reads
    // GameManager.CurrentDay and applies every entry with Day ≤ today: those GameObjects come
    // on, and PlayerShootingModule gets the highest WeaponsUnlocked value among them.
    public class DayUnlocks : MonoBehaviour
    {
        [Serializable]
        public class DayUnlock
        {
            public int Day;
            [Tooltip("Total weapons unlocked once this day begins (cumulative).")]
            public int WeaponsUnlocked = 1;
            [Tooltip("GameObjects that become active this day. Combine for turret combos: e.g. " +
                     "Day 3 = [Doner_02, PaperPlane_01, PaperPlane_02].")]
            public List<GameObject> Activate = new();
        }

        [SerializeField] private PlayerShootingModule _shootingModule;
        [SerializeField] private List<DayUnlock> _plan = new();

        private void Start()
        {
            int currentDay = GameManager.Instance != null ? GameManager.Instance.CurrentDay : 1;
            ApplyForDay(currentDay);
        }

        [Button]
        public void ApplyForDay(int day)
        {
            // Reset — any object mentioned anywhere in the plan starts disabled so re-runs are clean.
            foreach (var entry in _plan)
                if (entry != null)
                    foreach (var obj in entry.Activate)
                        if (obj != null) obj.SetActive(false);

            // Apply cumulatively up to `day`.
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
