using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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

        [Header("Shared sprites (consumed by paper bases via Active)")]
        [SerializeField] private List<Sprite> _apartmentSprites;
        [SerializeField] private List<Sprite> _studentSprites;

        [Header("Tablet slide-in (BeginGame)")]
        [Tooltip("Root of the left tablet (placed off-screen-left in editor). Slides to its target.")]
        [SerializeField] private RectTransform _apartmentScreenRoot;
        [Tooltip("Root of the right tablet (placed off-screen-right in editor). Slides to its target.")]
        [SerializeField] private RectTransform _studentScreenRoot;
        [Tooltip("World-space Transform where the apartment screen should land.")]
        [SerializeField] private Transform _apartmentScreenTarget;
        [Tooltip("World-space Transform where the student screen should land.")]
        [SerializeField] private Transform _studentScreenTarget;
        [SerializeField] private float _slideDuration = 0.5f;
        [SerializeField] private Ease _slideEase = Ease.OutQuad;

        [Header("Round flow")]
        [Tooltip("How many students are pre-loaded into the student slider on BeginGame. ≥2 keeps " +
                 "the slide-out → slide-in transition smooth (the next student is queued off-screen).")]
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
            var apartmentData = _apartmentPaperSlider.CurrentData;
            var studentData = _studentPaperSlider.CurrentData;

            var result = MatchValidator.Evaluate(studentData, apartmentData);
            Debug.Log($"[Match] {studentData.FullName} x {apartmentData.Name} — " +
                      $"Price:{result.PriceMatch} Anmeldung:{result.AnmeldungMatch} Dormitory:{result.DormitoryMatch} " +
                      $"Schufa: {result.SchufaMatch} " +
                      $"=> {(result.IsFullMatch ? "FULL MATCH" : $"{result.CorrectCount}/4")}");

            // Apartment recycles to the back of the strip — the player can swipe back to it.
            _apartmentPaperSlider.RecycleCurrent();

            // Student slides out and is replaced — next sequential, then random after the first pass.
            _studentPaperSlider.SlideOutCurrent(() =>
            {
                var next = PickNextStudent();
                if (next != null) _studentPaperSlider.AddPage(next);
            });
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
