using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Game.Scripts.UI.Screens
{
    // One full-screen day-intro image per day (Day 1..N). Play(day) hides all and activates the
    // matching image; the button on that image is wired to DayStartUI.Dismiss via Inspector
    // OnClick (every day's button can share the same handler — only one is active at a time).
    public class DayStartUI : UIScreen
    {
        [Serializable]
        public class DayStartImage
        {
            public int Day;
            public GameObject Root;
        }

        [SerializeField] private List<DayStartImage> _dayStarts = new();

        private bool _dismissed;
        private DayStartImage _activeEntry;

        public IEnumerator Play(int day)
        {
            foreach (var d in _dayStarts)
                if (d.Root != null) d.Root.SetActive(false);

            _activeEntry = _dayStarts.Find(d => d.Day == day);
            if (_activeEntry == null || _activeEntry.Root == null) yield break;
            _activeEntry.Root.SetActive(true);

            _dismissed = false;
            yield return Show().WaitForCompletion();
            while (!_dismissed) yield return null;
            yield return Hide().WaitForCompletion();

            _activeEntry.Root.SetActive(false);
            _activeEntry = null;
        }

        // Wire each day image's button's OnClick → DayStartUI.Dismiss in the Inspector.
        public void Dismiss() => _dismissed = true;
    }
}
