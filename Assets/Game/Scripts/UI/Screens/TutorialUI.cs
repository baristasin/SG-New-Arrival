using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

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
        [SerializeField] private float _dismissBlockDuration = 3f;

        private bool _dismissed;
        private bool _canDismiss;
        private TutorialImage _activeEntry;

        public IEnumerator Play(MinigameId id, System.Action onShown = null, System.Action onDismissed = null)
        {
            foreach (var t in _tutorials)
                if (t.Root != null) t.Root.SetActive(false);

            _activeEntry = _tutorials.Find(t => t.Id == id);
            if (_activeEntry == null || _activeEntry.Root == null) yield break;
            _activeEntry.Root.SetActive(true);

            _dismissed = false;
            _canDismiss = false;

            // Disable any buttons inside the tutorial root for the block window — visually shows
            // the player can't dismiss yet, and the Dismiss() guard below catches any code paths
            // that might bypass the button (e.g. queued clicks from spamming Complain).
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
