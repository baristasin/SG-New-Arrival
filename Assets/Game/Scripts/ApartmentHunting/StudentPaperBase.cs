using Game.Scripts.StudentData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.ApartmentHunting
{
    public class StudentPaperBase : HuntingPaperBase<StudentProfile>
    {
        [SerializeField] private Image _studentImage;
        
        [SerializeField] private TextMeshProUGUI _studentNameText;

        [SerializeField] private TextMeshProUGUI _budgetStoryText;

        [SerializeField] private TextMeshProUGUI _visaStoryText;

        [SerializeField] private TextMeshProUGUI _enrollmentStoryText;

        [SerializeField] private TextMeshProUGUI _schufaRecord;

        [SerializeField] private TextMeshProUGUI _nationality;
        [SerializeField] private TextMeshProUGUI _placeOfBirth;
        
        [SerializeField] private TextMeshProUGUI _dateOfBirth;

        public override void Initialize(StudentProfile data)
        {
            base.Initialize(data);
            _studentNameText.text = data.FullName;
            _budgetStoryText.text = data.Budget.ToString();
            _visaStoryText.text = data.VisaStatus.ToString();
            _enrollmentStoryText.text = data.IsEnrolled ? "Enrolled" : "Not Enrolled";
            _schufaRecord.text = data.HasPreviousSchufa ? "Yes schufa" : "No schufa";
            _nationality.text = data.Nationality;
            _dateOfBirth.text = data.DateOfBirth;
            _placeOfBirth.text = data.PlaceOfBirth;

            if (_studentImage != null && ApartmentHuntingManager.Active != null)
                _studentImage.sprite = ApartmentHuntingManager.Active.GetStudentSpriteForId(data.IdNumber);
        }
    }
}
