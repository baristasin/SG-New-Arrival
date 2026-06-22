using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Screens
{
    public class NightResultUI : UIScreen
    {
        [Header("Failed (player / building died)")]
        [SerializeField] private GameObject _failedRoot;
        [SerializeField] private TMP_Text _failedSanityText;
        [Tooltip("{0} = tomorrow's sanity percent (int).")]

        [Header("Success (all zombies eliminated)")]
        [SerializeField] private GameObject _successRoot;
        [SerializeField] private TMP_Text _successKillsText;
        [Tooltip("{0} = number of zombies eliminated (int).")]

        [Header("Shared")]
        [SerializeField] private Button _continueButton;

        private bool _dismissed;

        protected override void Awake()
        {
            base.Awake();
            if (_continueButton != null)
                _continueButton.onClick.AddListener(() => _dismissed = true);
        }

        public IEnumerator PlayFail(int sanityPercent, Action onShown = null, Action onDismissed = null)
        {
            SetActiveState(failed: true);
            _failedSanityText.text = sanityPercent.ToString();
            yield return RunDismissCycle(onShown, onDismissed);
        }

        public IEnumerator PlaySuccess(int killCount, Action onShown = null, Action onDismissed = null)
        {
            SetActiveState(failed: false);
            _successKillsText.text = killCount.ToString();
            yield return RunDismissCycle(onShown, onDismissed);
        }

        private void SetActiveState(bool failed)
        {
            if (_failedRoot  != null) _failedRoot.SetActive(failed);
            if (_successRoot != null) _successRoot.SetActive(!failed);
        }

        private IEnumerator RunDismissCycle(Action onShown, Action onDismissed)
        {
            _dismissed = false;
            yield return Show().WaitForCompletion();
            onShown?.Invoke();
            while (!_dismissed) yield return null;
            onDismissed?.Invoke();
            yield return Hide().WaitForCompletion();
        }

        public void Dismiss() => _dismissed = true;
    }
}
