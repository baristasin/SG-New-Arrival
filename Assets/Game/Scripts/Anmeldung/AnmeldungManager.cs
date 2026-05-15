using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Anmeldung
{
    public enum PaperItemCategory
    {
        NameAndSurname,
        Date,
        Signature,
        Photo,
        Fingerprint
    }
    
    [Serializable]
    public class AnmeldungData
    {
        public AnmeldungDocument AnmeldungDocument;
        public List<DraggableItemData> DraggableItemDatas;
    }

    public class AnmeldungManager : MonoBehaviour
    {
        [SerializeField] private List<AnmeldungData> _anmeldungDatas;
        [SerializeField] private DraggableItem _draggableItemPrefab;
        [SerializeField] private RectTransform _paperParent;
        [SerializeField] private List<RectTransform> _itemTransforms;
        
        [SerializeField] private float _slideDistance = 800f;
        [SerializeField] private float _slideDuration = 0.4f;

        [SerializeField] private Camera _mainCamera;
        [SerializeField] private Canvas _canvas;

        private AnmeldungDocument _currentAnmeldung;
        private List<DraggableItem> _currentItems;
        private DropSlot[] _currentSlots;
        private int _currentRound;

        private int _points;

        private void Start()
        {
            StartCoroutine(StartRound());
        }

        private IEnumerator StartRound()
        {
            yield return SlideInAnmeldungPaper();
            yield return SlideInItems();
        }

        private IEnumerator SlideInAnmeldungPaper()
        {
            _currentAnmeldung = Instantiate(_anmeldungDatas[_currentRound].AnmeldungDocument, _paperParent);
            _currentAnmeldung.transform.localPosition = Vector3.left * _slideDistance;

            yield return _currentAnmeldung.transform.DOLocalMove(Vector3.zero, _slideDuration)
                .SetEase(Ease.OutQuad).WaitForCompletion();

            _currentSlots = _currentAnmeldung.Slots;
        }

        private IEnumerator SlideInItems()
        {
            var datas = _anmeldungDatas[_currentRound].DraggableItemDatas;
            _currentItems = new List<DraggableItem>();

            for (int i = 0; i < datas.Count; i++)
            {
                var draggableItem = Instantiate(_draggableItemPrefab, _itemTransforms[i]);
                draggableItem.Initialize(_mainCamera,_canvas, datas[i]);
                _currentItems.Add(draggableItem);

                draggableItem.transform.localPosition = Vector3.right * _slideDistance;
                draggableItem.transform.DOLocalMove(Vector3.zero, _slideDuration)
                    .SetEase(Ease.OutQuad).SetDelay(i * 0.05f);
            }

            yield return new WaitForSeconds(_slideDuration + datas.Count * 0.05f);
        }

        private IEnumerator SlideOutAll()
        {
            if (_currentAnmeldung != null)
            {
                _currentAnmeldung.StopEffects();
                _currentAnmeldung.transform.DOLocalMove(Vector3.left * _slideDistance, _slideDuration)
                    .SetEase(Ease.InQuad);
            }

            if (_currentItems != null)
            {
                for (int i = 0; i < _currentItems.Count; i++)
                {
                    if (_currentItems[i] == null) continue;
                    if (_currentItems[i].IsInSlot) continue;
                    _currentItems[i].transform.DOLocalMove(Vector3.right * _slideDistance, _slideDuration)
                        .SetEase(Ease.InQuad).SetDelay(i * 0.05f);
                }
            }

            yield return new WaitForSeconds(_slideDuration + (_currentItems?.Count ?? 0) * 0.05f);

            if (_currentAnmeldung != null) Destroy(_currentAnmeldung.gameObject);
            if (_currentItems != null)
            {
                foreach (var item in _currentItems)
                {
                    if (item != null) Destroy(item.gameObject);
                }
            }
        }
        
        [Button]
        public void CompletePaper()
        {
            CheckCompletion();
        }
        
        public void CheckCompletion()
        {
            foreach (var slot in _currentSlots)
            {
                if (slot.IsCorrect) _points++;
            }

            StartCoroutine(OnRoundComplete());
        }

        private IEnumerator OnRoundComplete()
        {
            yield return SlideOutAll();
            yield return StartRound();
        }
    }
}
