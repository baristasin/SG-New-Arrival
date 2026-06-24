using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMODUnity;
using Game.Scripts.StudentData;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.ApartmentHunting
{
    public class ApartmentHuntingManager : MonoBehaviour, IMinigameManager
    {
        public static ApartmentHuntingManager Active { get; private set; }

        [SerializeField] private StudentDatabase _studentDatabase;
        [SerializeField] private ApartmentDatabase _apartmentDatabase;
        [SerializeField] private ApartmentPaperSlider _apartmentPaperSlider;
        [SerializeField] private StudentPaperSlider _studentPaperSlider;

        [SerializeField] private List<Sprite> _apartmentSprites;
        [SerializeField] private List<Sprite> _studentSprites;

        [SerializeField] private EventReference _matchSubmitEvent;
        [SerializeField] private EventReference _tvBuzzingEvent;

        [SerializeField, Min(1)] private int _scoreRequired = 10;

        [SerializeField] private UnityEngine.UI.Button _matchButton;
        [SerializeField] private float _matchCooldown = 2f;
        
        private int _correctCriteriaTotal;
        public int GetScorePercent() =>
            Mathf.Clamp(Mathf.RoundToInt(100f * _correctCriteriaTotal / (4f * _scoreRequired)), 0, 100);

        public void PlayMatchSubmit() { if (!_matchSubmitEvent.IsNull) AudioManager.Instance.PlayOneShot(_matchSubmitEvent); }
        public EventReference TvBuzzingEvent => _tvBuzzingEvent;

        [SerializeField] private RectTransform _apartmentScreenRoot;
        [SerializeField] private RectTransform _studentScreenRoot;
        [SerializeField] private Transform _apartmentScreenTarget;
        [SerializeField] private Transform _studentScreenTarget;
        [SerializeField] private float _slideDuration = 0.5f;
        [SerializeField] private Ease _slideEase = Ease.OutQuad;

        [SerializeField] private int _studentInitialBufferSize = 2;
        
        private int _studentRound;

        public Sprite GetApartmentSpriteForId(int id) => LookupById(_apartmentSprites, id);
        public Sprite GetStudentSpriteForId(int id) => LookupById(_studentSprites, id);
        
        private static Sprite LookupById(List<Sprite> list, int id)
        {
            if (list == null || list.Count == 0) return null;
            int index = Mathf.Max(0, id - 1) % list.Count;
            return list[index];
        }

        private void Awake() => Active = this;
        private void OnDestroy() { if (Active == this) Active = null; }
        
        public void BeginGame()
        {
            StartCoroutine(BeginGameRoutine());
        }

        private IEnumerator BeginGameRoutine()
        {
            if (_apartmentScreenRoot != null && _apartmentScreenTarget != null)
                _apartmentScreenRoot.DOMove(_apartmentScreenTarget.position, _slideDuration).SetEase(_slideEase);

            if (_studentScreenRoot != null && _studentScreenTarget != null)
                _studentScreenRoot.DOMove(_studentScreenTarget.position, _slideDuration).SetEase(_slideEase);

            yield return new WaitForSeconds(_slideDuration);
            Initialize();
        }

        [Button]
        public void Initialize()
        {
            _studentRound = 0;
            _correctCriteriaTotal = 0;
            StartCoroutine(LockMatch());
            
            _apartmentPaperSlider.Initialize(_apartmentDatabase.Apartments);
            
            var initialStudents = new List<StudentProfile>(_studentInitialBufferSize);
            for (int i = 0; i < _studentInitialBufferSize; i++)
            {
                var st = PickNextStudent();
                if (st != null) initialStudents.Add(st);
            }
            _studentPaperSlider.Initialize(initialStudents);
        }

        public void MatchClicked()
        {
            StartCoroutine(LockMatch());
            PlayMatchSubmit();

            var apartmentData = _apartmentPaperSlider.CurrentData;
            var studentData = _studentPaperSlider.CurrentData;

            var result = MatchValidator.Evaluate(studentData, apartmentData);
            _correctCriteriaTotal += result.CorrectCount;   

            _apartmentPaperSlider.RecycleCurrent();

            _studentPaperSlider.SlideOutCurrent(() =>
            {
                var next = PickNextStudent();
                if (next != null) _studentPaperSlider.AddPage(next);
            });
        }

        private IEnumerator LockMatch()
        {
            if (_matchButton == null) yield break;
            _matchButton.interactable = false;
            yield return new WaitForSeconds(_matchCooldown);
            _matchButton.interactable = true;
        }

        private StudentProfile PickNextStudent()
        {
            var list = _studentDatabase != null ? _studentDatabase.Students : null;
            if (list == null || list.Count == 0) return null;
            var pick = _studentRound < list.Count
                ? list[_studentRound]
                : list[Random.Range(0, list.Count)];
            _studentRound++;
            return pick;
        }
    }
}
