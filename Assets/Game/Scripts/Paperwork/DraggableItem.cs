using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Scripts.Paperwork
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
            _itemText.SetText(draggableItemData.ItemDataStr);
            _originalPosition = Vector3.zero;
            _originalParent = transform.parent;
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
            _rectTransform.DOLocalMove(Vector3.zero, 0.15f).SetEase(Ease.OutQuad);
            _rectTransform.DOLocalRotate(Vector3.zero, 0.15f);
        }

        public void ReturnToOriginal()
        {
            transform.SetParent(_originalParent);
            _rectTransform.DOLocalMove(_originalPosition, 0.2f).SetEase(Ease.OutBack);
        }
    }
}
