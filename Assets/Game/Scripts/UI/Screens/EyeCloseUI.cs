using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Game.Scripts.UI.Screens
{

    public class EyeCloseUI : UIScreen
    {
        [SerializeField] private RectTransform _topPanel;
        [SerializeField] private RectTransform _bottomPanel;
        [SerializeField] private float _closeDuration = 1.2f;
        [SerializeField] private float _openDuration  = 0.8f;
        [SerializeField] private Ease _closeEase = Ease.InQuad;
        [SerializeField] private Ease _openEase  = Ease.OutQuad;

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
        
        public IEnumerator Open()
        {
            if (_topPanel    != null) _topPanel.DOAnchorPosY(_topOpenPos.y,    _openDuration).SetEase(_openEase);
            if (_bottomPanel != null) _bottomPanel.DOAnchorPosY(_bottomOpenPos.y, _openDuration).SetEase(_openEase);
            yield return new WaitForSeconds(_openDuration);
            Hide();
        }

        public void Dismiss() => _dismissed = true;

        private void ResetPanels()
        {
            if (_topPanel    != null) _topPanel.anchoredPosition    = _topOpenPos;
            if (_bottomPanel != null) _bottomPanel.anchoredPosition = _bottomOpenPos;
        }
    }
}
