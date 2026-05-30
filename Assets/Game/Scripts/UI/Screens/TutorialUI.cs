using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Game.Scripts.UI.Screens
{
    // One full-screen tutorial image per minigame. Play(id) hides all and activates the matching
    // image; the close button INSIDE that image is wired to TutorialUI.Dismiss via Inspector
    // OnClick (all three buttons can share the same handler — only one image is ever active).
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

        // onShown fires after the fade-in — caller hides the Loading cover here.
        // onDismissed fires after the click but before the fade-out — caller can raise Loading
        // again if the next step needs it.
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

        // Wire each close button's OnClick → TutorialUI.Dismiss in the Inspector.
        public void Dismiss() => _dismissed = true;
    }
}
