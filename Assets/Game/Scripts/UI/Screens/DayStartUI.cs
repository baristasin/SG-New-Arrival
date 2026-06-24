using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Screens
{
    public class DayStartUI : UIScreen
    {
        [Serializable]
        public class DayStartImage
        {
            public int Day;
            public GameObject Root;
        }

        [SerializeField] private List<DayStartImage> _dayStarts = new();
        [SerializeField] private float _dismissBlockDuration = 3f;

        private bool _dismissed;
        private bool _canDismiss;
        private DayStartImage _activeEntry;

        public IEnumerator Play(int day, System.Action onShown = null, System.Action onDismissed = null)
        {
            foreach (var d in _dayStarts)
                if (d.Root != null) d.Root.SetActive(false);

            _activeEntry = _dayStarts.Find(d => d.Day == day);
            if (_activeEntry == null || _activeEntry.Root == null) yield break;
            _activeEntry.Root.SetActive(true);

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
