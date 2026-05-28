using System;
using System.Collections.Generic;
using Game.Scripts.StudentData;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.ApartmentHunting
{
    public class ApartmentHuntingManager : MonoBehaviour
    {
        public static ApartmentHuntingManager Active { get; private set; }

        [SerializeField] private StudentDatabase _studentDatabase;
        [SerializeField] private ApartmentDatabase _apartmentDatabase;
        [SerializeField] private ApartmentPaperSlider _apartmentPaperSlider;
        [SerializeField] private StudentPaperSlider _studentPaperSlider;

        [Header("Shared sprites (consumed by paper bases via Active)")]
        [SerializeField] private List<Sprite> _apartmentSprites;
        [SerializeField] private List<Sprite> _studentSprites;

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

        private void Start()
        {
            Initialize();
        }

        [Button]
        public void Initialize()
        {
            _apartmentPaperSlider.Initialize(_apartmentDatabase.Apartments);
            _studentPaperSlider.Initialize(_studentDatabase.Students);
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

            _apartmentPaperSlider.SlideOutCurrent();
            _studentPaperSlider.SlideOutCurrent();
        }
    }
}
