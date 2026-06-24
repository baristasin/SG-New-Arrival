using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Game.Scripts.UI.Screens
{

    public class DayRewardsUI : UIScreen
    {
        [Serializable]
        public class DayRewardImage
        {
            public int Day;
            public GameObject Root;
        }

        [SerializeField] private List<DayRewardImage> _rewards = new();

        [SerializeField] private TMP_Text _scoreText;

        private bool _dismissed;
        private DayRewardImage _activeEntry;
        
        public IEnumerator Play(int day, int scorePercent, System.Action onShown = null, System.Action onDismissed = null)
        {
            foreach (var r in _rewards)
                if (r.Root != null) r.Root.SetActive(false);

            _activeEntry = _rewards.Find(r => r.Day == day);
            if (_activeEntry == null || _activeEntry.Root == null) yield break;
            _activeEntry.Root.SetActive(true);

            if (_scoreText != null) _scoreText.text = scorePercent.ToString();

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
