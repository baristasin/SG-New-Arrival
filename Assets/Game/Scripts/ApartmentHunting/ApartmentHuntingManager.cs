using Game.Scripts.ApartmentHunting.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.ApartmentHunting
{
    public class ApartmentHuntingManager : MonoBehaviour
    {
        [SerializeField] private ApartmentPaperSlider _apartmentPaperSlider;
        [SerializeField] private StudentPaperSlider _studentPaperSlider;

        [Button]
        public void Initialize()
        {
            var apartments = ApartmentHuntingDataLoader.LoadApartments();
            var students = ApartmentHuntingDataLoader.LoadStudents();

            _apartmentPaperSlider.Initialize(apartments);
            _studentPaperSlider.Initialize(students);
        }

        public void MatchClicked()
        {
            var apartmentData = _apartmentPaperSlider.CurrentData;
            var studentData = _studentPaperSlider.CurrentData;
            
            var result = MatchValidator.Evaluate(studentData, apartmentData);
            
            _apartmentPaperSlider.SlideOutCurrent();
            _studentPaperSlider.SlideOutCurrent();
        }
    }
}