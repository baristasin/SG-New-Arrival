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

        private int _fullMatchCount;
        public int GetScorePercent() =>
            Mathf.Clamp(Mathf.RoundToInt(100f * _fullMatchCount / _scoreRequired), 0, 100);

        public void PlayMatchSubmit() { if (!_matchSubmitEvent.IsNull) AudioManager.Instance.PlayOneShot(_matchSubmitEvent); }
        public EventReference TvBuzzingEvent => _tvBuzzingEvent;

        [SerializeField] private RectTransform _apartmentScreenRoot;
        [SerializeField] private RectTransform _studentScreenRoot;
        [SerializeField] private Transform _apartmentScreenTarget;
        [SerializeField] private Transform _studentScreenTarget;
        [SerializeField] private float _slideDuration = 0.5f;
        [SerializeField] private Ease _slideEase = Ease.OutQuad;

        [SerializeField] private int _studentInitialBufferSize = 2;

        // Sequential counter for student picks; once we've gone through every entry once, the
        // picker switches to random. Apartments don't have a counter — the strip holds the full
        // list and matched ones get recycled to the back.
        private int _studentRound;

        public Sprite GetApartmentSpriteForId(int id) => LookupById(_apartmentSprites, id);
        public Sprite GetStudentSpriteForId(int id) => LookupById(_studentSprites, id);

        // index = id - 1 (so Id 1 maps to element 0); wraps with modulo if the id is past the
        // end of the list, so we always return something.
        private static Sprite LookupById(List<Sprite> list, int id)
        {
            if (list == null || list.Count == 0) return null;
            int index = Mathf.Max(0, id - 1) % list.Count;
            return list[index];
        }

        private void Awake() => Active = this;
        private void OnDestroy() { if (Active == this) Active = null; }

        // Called by MinigameStation after the tutorial closes. Slides the two tablet screens
        // from their off-screen editor positions to the assigned target Transforms, then sets
        // up paper data.
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
            _fullMatchCount = 0;
            StartCoroutine(LockMatch());

            // Apartments: load the full list so the player can swipe through every option.
            // Matched ones get recycled to the back in MatchClicked, never destroyed.
            _apartmentPaperSlider.Initialize(_apartmentDatabase.Apartments);

            // Students: small buffer + refill on slide-out. Sequential through the database
            // for the first pass, then random.
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
            if (result.IsFullMatch) _fullMatchCount++;

            // Apartment recycles to the back of the strip — the player can swipe back to it.
            _apartmentPaperSlider.RecycleCurrent();

            // Student slides out and is replaced — next sequential, then random after the first pass.
            _studentPaperSlider.SlideOutCurrent(() =>
            {
                var next = PickNextStudent();
                if (next != null) _studentPaperSlider.AddPage(next);
            });
        }

        // Disables the Match button for _matchCooldown seconds so the player can't spam-match
        // while papers slide.
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
