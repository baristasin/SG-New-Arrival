using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Game.Scripts.UI.Screens
{
    public class TutorialUI : UIScreen
    {
        [Serializable]
        public class TutorialImage
        {
            public MinigameId Id;
            public GameObject Root;
        }

        [SerializeField] private List<TutorialImage> _tutorials = new();

        private bool _dismissed;
        private TutorialImage _activeEntry;
        
        public IEnumerator Play(MinigameId id, System.Action onShown = null, System.Action onDismissed = null)
        {
            foreach (var t in _tutorials)
                if (t.Root != null) t.Root.SetActive(false);

            _activeEntry = _tutorials.Find(t => t.Id == id);
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
