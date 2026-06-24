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
        [SerializeField] private GameObject _failedRoot;
        [SerializeField] private TMP_Text _failedSanityText;

        [SerializeField] private GameObject _successRoot;
        [SerializeField] private TMP_Text _successKillsText;

        [SerializeField] private Button _continueButton;
        [SerializeField] private float _dismissBlockDuration = 3f;

        private bool _dismissed;
        private bool _canDismiss;

        protected override void Awake()
        {
            base.Awake();
            if (_continueButton != null)
                _continueButton.onClick.AddListener(Dismiss);
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
            _canDismiss = false;
            if (_continueButton != null) _continueButton.interactable = false;

            yield return Show().WaitForCompletion();
            onShown?.Invoke();

            yield return new WaitForSeconds(_dismissBlockDuration);
            _canDismiss = true;
            if (_continueButton != null) _continueButton.interactable = true;

            while (!_dismissed) yield return null;
            onDismissed?.Invoke();
            yield return Hide().WaitForCompletion();
        }

        public void Dismiss()
        {
            if (!_canDismiss) return;
            _dismissed = true;
        }
    }
}
