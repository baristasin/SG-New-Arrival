using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Game.Scripts.UI.Screens
{
    // Sanity-0 sleep transition. Two opaque panels (top + bottom) slide toward the middle until
    // the screen is fully black, then a fullscreen "why we're going to night" explanation panel
    // appears on top of the black. The player clicks a button on that panel (wired to Dismiss
    // via Inspector OnClick) and Play returns so the caller can continue into the loading screen
    // for NightCity.
    public class EyeCloseUI : UIScreen
    {
        [Header("Eyelid panels")]
        [SerializeField] private RectTransform _topPanel;
        [SerializeField] private RectTransform _bottomPanel;
        [SerializeField] private float _closeDuration = 1.2f;
        [SerializeField] private float _openDuration  = 0.8f;
        [SerializeField] private Ease _closeEase = Ease.InQuad;
        [SerializeField] private Ease _openEase  = Ease.OutQuad;

        [Header("Post-close explanation panel")]
        [Tooltip("Fullscreen GameObject (image + text + button) shown after the eyelids close. " +
                 "Wire its button's OnClick to EyeCloseUI.Dismiss.")]
        [SerializeField] private GameObject _explanationRoot;

        private Vector2 _topOpenPos;
        private Vector2 _bottomOpenPos;
        private bool _dismissed;

        protected override void Awake()
        {
            base.Awake();
            if (_topPanel    != null) _topOpenPos    = _topPanel.anchoredPosition;
            if (_bottomPanel != null) _bottomOpenPos = _bottomPanel.anchoredPosition;
            if (_explanationRoot != null) _explanationRoot.SetActive(false);
        }

        // Close eyelids → reveal explanation → wait for click → return. Panels stay closed
        // and the screen stays shown; the caller is expected to bring up a loading screen and
        // then Hide() this one once it's covered.
        public IEnumerator Play()
        {
            ResetPanels();
            _dismissed = false;
            if (_explanationRoot != null) _explanationRoot.SetActive(false);
            Show();

            if (_topPanel    != null) _topPanel.DOAnchorPosY(0f, _closeDuration).SetEase(_closeEase);
            if (_bottomPanel != null) _bottomPanel.DOAnchorPosY(0f, _closeDuration).SetEase(_closeEase);
            yield return new WaitForSeconds(_closeDuration);

            if (_explanationRoot != null) _explanationRoot.SetActive(true);
            while (!_dismissed) yield return null;
            if (_explanationRoot != null) _explanationRoot.SetActive(false);
        }

        // Open the eyelids back up. Currently unused in the production flow (the night loading
        // screen covers everything) but kept for debug / future "wake up" moments.
        public IEnumerator Open()
        {
            if (_topPanel    != null) _topPanel.DOAnchorPosY(_topOpenPos.y,    _openDuration).SetEase(_openEase);
            if (_bottomPanel != null) _bottomPanel.DOAnchorPosY(_bottomOpenPos.y, _openDuration).SetEase(_openEase);
            yield return new WaitForSeconds(_openDuration);
            Hide();
        }

        // Wire the explanation panel's continue button to this method via Inspector OnClick.
        public void Dismiss() => _dismissed = true;

        private void ResetPanels()
        {
            if (_topPanel    != null) _topPanel.anchoredPosition    = _topOpenPos;
            if (_bottomPanel != null) _bottomPanel.anchoredPosition = _bottomOpenPos;
        }
    }
}
