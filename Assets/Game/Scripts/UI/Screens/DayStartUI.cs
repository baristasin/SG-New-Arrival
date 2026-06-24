using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

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

        private bool _dismissed;
        private DayStartImage _activeEntry;


        public IEnumerator Play(int day, System.Action onShown = null, System.Action onDismissed = null)
        {
            foreach (var d in _dayStarts)
                if (d.Root != null) d.Root.SetActive(false);

            _activeEntry = _dayStarts.Find(d => d.Day == day);
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

        public void Dismiss() => _dismissed = true;
    }
}
