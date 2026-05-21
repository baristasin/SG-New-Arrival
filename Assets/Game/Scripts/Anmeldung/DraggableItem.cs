using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Scripts.Anmeldung
{
    [Serializable]
    public class DraggableItemData
    {
        public PaperItemCategory PaperItemCategory;
        public string ItemDataStr;
    }
    
    public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public DraggableItemData DraggableItemData { get; private set; }

        [SerializeField] private TextMeshProUGUI _itemText;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private CanvasGroup _canvasGroup;

        [SerializeField] private float _horizontalPadding = 40f;
        [SerializeField] private float _minWidth = 120f;
        [SerializeField] private float _maxWidth = 400f;

        private Canvas _canvas;
        private Vector3 _originalPosition;
        private Transform _originalParent;
        private DropSlot _currentSlot;
        public bool IsInSlot => _currentSlot != null;
        private Camera _mainCamera;

        public void Initialize(Camera mainCamera,Canvas canvas ,DraggableItemData draggableItemData)
        {
            DraggableItemData = draggableItemData;
            _mainCamera = mainCamera;
            _canvas = canvas;
            _itemText.enableAutoSizing = false;
            _itemText.SetText(draggableItemData.ItemDataStr);
            FitWidthToText(draggableItemData.ItemDataStr);
            _originalPosition = Vector3.zero;
            _originalParent = transform.parent;
        }

        private void FitWidthToText(string text)
        {
            var oldRootSize = _rectTransform.sizeDelta;
            float preferredTextWidth = _itemText.GetPreferredValues(text).x;
            float desiredRootWidth = preferredTextWidth + _horizontalPadding;

            float targetWidth = Mathf.Min(_maxWidth, Mathf.Max(_minWidth, desiredRootWidth));

            float widthDelta = targetWidth - oldRootSize.x;
            _rectTransform.sizeDelta = new Vector2(targetWidth, oldRootSize.y);

            var textRect = _itemText.rectTransform;
            var textSize = textRect.sizeDelta;
            textRect.sizeDelta = new Vector2(textSize.x + widthDelta, textSize.y);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_currentSlot != null)
            {
                _currentSlot.Clear();
                _currentSlot = null;
            }

            transform.SetParent(_canvas.transform);
            _canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Ray ray = _mainCamera.ScreenPointToRay(eventData.position);
            Plane plane = new Plane(_canvas.transform.forward, _canvas.transform.position);
            if (plane.Raycast(ray, out float distance))
                _rectTransform.position = ray.GetPoint(distance);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _canvasGroup.blocksRaycasts = true;

            if (transform.parent == _canvas.transform)
                ReturnToOriginal();
        }

        public void SnapToSlot(DropSlot slot)
        {
            _currentSlot = slot;
            transform.SetParent(slot.transform);
            _rectTransform.DOLocalMove(new Vector3(0,8f,0), 0.15f).SetEase(Ease.OutQuad);
            _rectTransform.DOLocalRotate(Vector3.zero, 0.15f);
        }

        public void ReturnToOriginal()
        {
            transform.SetParent(_originalParent);
            _rectTransform.DOLocalMove(_originalPosition, 0.2f).SetEase(Ease.OutBack);
        }
    }
}
