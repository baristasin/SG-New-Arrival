using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Game.Scripts.UI.Screens
{
    // One full-screen rewards image per day (Day 1..N). Play(day) hides all and activates the
    // matching image; the close button inside that image is wired to DayRewardsUI.Dismiss via
    // Inspector OnClick (every day's button can share the same handler — only one is active).
    public class DayRewardsUI : UIScreen
    {
        [Serializable]
        public class DayRewardImage
        {
            public int Day;
            public GameObject Root;
        }

        [SerializeField] private List<DayRewardImage> _rewards = new();

        private bool _dismissed;
        private DayRewardImage _activeEntry;

        // onShown fires after fade-in — caller hides Loading cover here.
        // onDismissed fires after click but before fade-out — caller raises Loading cover here
        // so the fade-out happens behind cover (no underlying scene flash).
        public IEnumerator Play(int day, System.Action onShown = null, System.Action onDismissed = null)
        {
            foreach (var r in _rewards)
                if (r.Root != null) r.Root.SetActive(false);

            _activeEntry = _rewards.Find(r => r.Day == day);
            if (_activeEntry == null || _activeEntry.Root == null) yield break;
            _activeEntry.Root.SetActive(true);

            _dismissed = false;
            yield return Show().WaitForCompletion();
            onShown?.Invoke();
            while (!_dismissed) yield return null;
            onDismissed?.Invoke();
            yield return Hide().WaitForCompletion();

            _activeEntry.Root.SetActive(false);
            _activeEntry = null;
        }

        // Wire each close button's OnClick → DayRewardsUI.Dismiss in the Inspector.
        public void Dismiss() => _dismissed = true;
    }
}
