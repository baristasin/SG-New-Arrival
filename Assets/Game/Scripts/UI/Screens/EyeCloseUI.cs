using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Game.Scripts.UI.Screens
{
    // Sanity-0 sleep transition: two opaque panels (top + bottom) slide toward the middle until
    // they meet and the screen is fully black. Reopen is used optionally — most callers leave it
    // closed and rely on the next screen (DayRewards) to fade in on top.
    public class EyeCloseUI : UIScreen
    {
        [Header("Eyelid panels")]
        [SerializeField] private RectTransform _topPanel;
        [SerializeField] private RectTransform _bottomPanel;
        [SerializeField] private float _closeDuration = 1.2f;
        [SerializeField] private float _openDuration  = 0.8f;
        [SerializeField] private Ease _closeEase = Ease.InQuad;
        [SerializeField] private Ease _openEase  = Ease.OutQuad;

        private Vector2 _topOpenPos;
        private Vector2 _bottomOpenPos;

        protected override void Awake()
        {
            base.Awake();
            if (_topPanel    != null) _topOpenPos    = _topPanel.anchoredPosition;
            if (_bottomPanel != null) _bottomOpenPos = _bottomPanel.anchoredPosition;
        }

        // Close to fully black. Panels meet at y = 0 (eyelids closed). Caller yields on this.
        public IEnumerator Play()
        {
            ResetPanels();
            Show();

            if (_topPanel    != null) _topPanel.DOAnchorPosY(0f, _closeDuration).SetEase(_closeEase);
            if (_bottomPanel != null) _bottomPanel.DOAnchorPosY(0f, _closeDuration).SetEase(_closeEase);

            yield return new WaitForSeconds(_closeDuration);
        }

        // Open from black. Panels return to their authored open positions, then the screen hides.
        public IEnumerator Open()
        {
            if (_topPanel    != null) _topPanel.DOAnchorPosY(_topOpenPos.y,    _openDuration).SetEase(_openEase);
            if (_bottomPanel != null) _bottomPanel.DOAnchorPosY(_bottomOpenPos.y, _openDuration).SetEase(_openEase);

            yield return new WaitForSeconds(_openDuration);
            Hide();
        }

        private void ResetPanels()
        {
            if (_topPanel    != null) _topPanel.anchoredPosition    = _topOpenPos;
            if (_bottomPanel != null) _bottomPanel.anchoredPosition = _bottomOpenPos;
        }
    }
}
