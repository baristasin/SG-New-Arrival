using Game.Scripts.StudentData;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.ApartmentHunting
{
    public class ApartmentHuntingManager : MonoBehaviour
    {
        [SerializeField] private StudentDatabase _studentDatabase;
        [SerializeField] private ApartmentDatabase _apartmentDatabase;
        [SerializeField] private ApartmentPaperSlider _apartmentPaperSlider;
        [SerializeField] private StudentPaperSlider _studentPaperSlider;

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
                      $"=> {(result.IsFullMatch ? "FULL MATCH" : $"{result.CorrectCount}/3")}");

            _apartmentPaperSlider.SlideOutCurrent();
            _studentPaperSlider.SlideOutCurrent();
        }
    }
}
