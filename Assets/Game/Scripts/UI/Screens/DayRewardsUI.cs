using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        [SerializeField] private float _dismissBlockDuration = 3f;

        private bool _dismissed;
        private bool _canDismiss;
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
            _canDismiss = false;

            var buttons = _activeEntry.Root.GetComponentsInChildren<Button>(true);
            foreach (var b in buttons) if (b != null) b.interactable = false;

            yield return Show().WaitForCompletion();
            onShown?.Invoke();

            yield return new WaitForSeconds(_dismissBlockDuration);
            _canDismiss = true;
            foreach (var b in buttons) if (b != null) b.interactable = true;

            while (!_dismissed) yield return null;
            onDismissed?.Invoke();
            yield return Hide().WaitForCompletion();

            _activeEntry.Root.SetActive(false);
            _activeEntry = null;
        }

        public void Dismiss()
        {
            if (!_canDismiss) return;
            _dismissed = true;
        }
    }
}
