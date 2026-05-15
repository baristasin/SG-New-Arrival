using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Scripts.Paperwork
{
    public class DropSlot : MonoBehaviour, IDropHandler
    {
        public PaperItemCategory ExpectedCategoryID;

        private DraggableItem _currentItem;

        public bool IsCorrect => _currentItem != null && _currentItem.DraggableItemData.PaperItemCategory == ExpectedCategoryID;
        public bool IsOccupied => _currentItem != null;

        public void OnDrop(PointerEventData eventData)
        {
            var incoming = eventData.pointerDrag?.GetComponent<DraggableItem>();
            if (incoming == null) return;

            if (_currentItem != null)
                _currentItem.ReturnToOriginal();

            _currentItem = incoming;
            _currentItem.SnapToSlot(this);
        }

        public void Clear()
        {
            _currentItem = null;
        }
    }
}
