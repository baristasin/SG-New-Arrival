using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game.Scripts.StudentData;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Anmeldung
{
    public class AnmeldungManager : MonoBehaviour, IMinigameManager
    {
        [SerializeField] private StudentDatabase _studentDatabase;
        [SerializeField] private List<AnmeldungDocument> _anmeldungDocuments;
        [SerializeField] private DraggableItem _draggableItemPrefab;
        [SerializeField] private RectTransform _paperParent;
        [SerializeField] private List<RectTransform> _itemTransforms;

        [SerializeField] private float _slideDistance = 800f;
        [SerializeField] private float _slideDuration = 0.4f;

        [SerializeField] private Camera _mainCamera;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private AnmeldungSanityCorruption _sanityCorruption;

        private AnmeldungDocument _currentAnmeldung;
        private List<DraggableItem> _currentItems;
        private DropSlot[] _currentSlots;
        private StudentProfile _currentStudent;
        private int _currentRound;

        private int _points;

        // Called by MinigameStation after the tutorial closes. Paper + items slide in here, not
        // on Start, so they don't appear behind the fullscreen tutorial sheet.
        public void BeginGame()
        {
            _currentRound = 0;
            _points = 0;
            StartCoroutine(StartRound());
        }

        private IEnumerator StartRound()
        {
            _currentStudent = PickStudentForRound(_currentRound);
            if (_currentStudent == null)
            {
                Debug.LogError("[AnmeldungManager] No student available — assign a StudentDatabase with entries.");
                yield break;
            }

            yield return SlideInAnmeldungPaper();
            yield return SlideInItems();
        }

        // Sequential for the first pass through the database, then random for every round after
        // — sanity (not the deck) decides when the day ends, so we never run out of students.
        private StudentProfile PickStudentForRound(int round)
        {
            var list = _studentDatabase != null ? _studentDatabase.Students : null;
            if (list == null || list.Count == 0) return null;
            return round < list.Count ? list[round] : list[Random.Range(0, list.Count)];
        }

        private IEnumerator SlideInAnmeldungPaper()
        {
            _currentAnmeldung = Instantiate(_anmeldungDocuments[_currentRound % _anmeldungDocuments.Count], _paperParent);
            _currentAnmeldung.transform.localPosition = Vector3.left * _slideDistance;

            yield return _currentAnmeldung.transform.DOLocalMove(Vector3.zero, _slideDuration)
                .SetEase(Ease.OutQuad).WaitForCompletion();

            _currentSlots = _currentAnmeldung.Slots;

            if (_sanityCorruption != null)
                _sanityCorruption.SetPaper(_currentAnmeldung);
        }

        private IEnumerator SlideInItems()
        {
            bool includeDistractors = _sanityCorruption == null || _sanityCorruption.ShouldShowDistractors;
            var datas = AnmeldungItemFactory.BuildRound(_currentStudent, includeDistractors);
            _currentItems = new List<DraggableItem>();

            int count = Mathf.Min(datas.Count, _itemTransforms.Count);
            if (datas.Count > _itemTransforms.Count)
                Debug.LogWarning($"[AnmeldungManager] {datas.Count} items but only {_itemTransforms.Count} spawn slots — extras dropped.");

            for (int i = 0; i < count; i++)
            {
                var draggableItem = Instantiate(_draggableItemPrefab, _itemTransforms[i]);
                draggableItem.Initialize(_mainCamera, _canvas, datas[i]);
                _currentItems.Add(draggableItem);

                draggableItem.transform.localPosition = Vector3.right * _slideDistance;
                draggableItem.transform.DOLocalMove(Vector3.zero, _slideDuration)
                    .SetEase(Ease.OutQuad).SetDelay(i * 0.05f);
            }

            yield return new WaitForSeconds(_slideDuration + count * 0.05f);
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
            var roundPoints = 0;
            
            foreach (var slot in _currentSlots)
            {
                if (slot.IsCorrect)
                {
                    roundPoints++;
                    _points++;
                }
            }
            
            Debug.Log($"Points: {roundPoints}/5");

            StartCoroutine(OnRoundComplete());
        }

        private IEnumerator OnRoundComplete()
        {
            yield return SlideOutAll();
            _currentRound++;
            yield return StartRound();
        }
    }
}
