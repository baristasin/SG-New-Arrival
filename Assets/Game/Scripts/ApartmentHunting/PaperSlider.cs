using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace Game.Scripts.ApartmentHunting
{
    public class PaperSlider<TPaper, TData> : MonoBehaviour
        where TPaper : HuntingPaperBase<TData>
    {
        [SerializeField] private RectTransform _viewport;
        [SerializeField] private RectTransform _pageParent;
        [SerializeField] private float _slideDuration = 0.3f;
        [SerializeField] private TPaper _paperPrefab;

        private readonly List<TPaper> _pages = new();
        private int _currentIndex;
        private float _pageWidth;
        private bool _isSliding;

        // The masked container that holds and clips the pages. Falls back to the
        // viewport when no dedicated parent is assigned.
        private RectTransform PageParent => _pageParent != null ? _pageParent : _viewport;

        public TData CurrentData => _pages.Count > 0 ? _pages[_currentIndex].Data : default;

        public void Initialize(List<TData> pageDatas)
        {
            _pageWidth = PageParent.rect.width;
            SetPages(pageDatas);
        }

        public void SetPages(List<TData> pageDatas)
        {
            _pages.Clear();
            _currentIndex = 0;

            for (int i = 0; i < pageDatas.Count; i++)
            {
                var paper = Instantiate(_paperPrefab, PageParent);
                paper.Initialize(pageDatas[i]);
                _pages.Add(paper);
                paper.RectTransform.anchoredPosition = new Vector2(i * _pageWidth, 0f);
            }
        }

        public void Next()
        {
            if (_isSliding || _currentIndex >= _pages.Count - 1) return;
            SlideTo(_currentIndex + 1);
        }

        public void Previous()
        {
            if (_isSliding || _currentIndex <= 0) return;
            SlideTo(_currentIndex - 1);
        }

        public void SlideTo(int index)
        {
            if (_isSliding || index < 0 || index >= _pages.Count) return;

            _isSliding = true;
            _currentIndex = index;

            for (int i = 0; i < _pages.Count; i++)
            {
                float targetX = (i - _currentIndex) * _pageWidth;
                _pages[i].RectTransform.DOAnchorPosX(targetX, _slideDuration).SetEase(Ease.OutQuad);
            }

            DOVirtual.DelayedCall(_slideDuration, () => _isSliding = false);
        }

        public void SlideOutCurrent(System.Action onComplete = null)
        {
            if (_isSliding || _pages.Count == 0) return;

            _isSliding = true;
            var current = _pages[_currentIndex];

            current.RectTransform.DOAnchorPosX(-_pageWidth, _slideDuration).SetEase(Ease.InQuad)
                .OnComplete(() =>
                {
                    _pages.Remove(current);
                    Destroy(current.gameObject);
                    _isSliding = false;

                    if (_currentIndex >= _pages.Count)
                        _currentIndex = _pages.Count - 1;

                    for (int i = 0; i < _pages.Count; i++)
                    {
                        float targetX = (i - _currentIndex) * _pageWidth;
                        _pages[i].RectTransform.DOAnchorPosX(targetX, _slideDuration).SetEase(Ease.OutQuad);
                    }

                    onComplete?.Invoke();
                });
        }
    }
}
