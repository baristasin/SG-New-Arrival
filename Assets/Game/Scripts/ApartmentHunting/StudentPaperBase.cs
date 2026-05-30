using Game.Scripts.StudentData;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.Scripts.ApartmentHunting
{
    public class StudentPaperBase : HuntingPaperBase<StudentProfile>
    {
        [SerializeField] private Image _studentImage;

        [SerializeField] private TextMeshProUGUI _studentNameText;

        [SerializeField] private TextMeshProUGUI _budgetStoryText;

        // Was _visaStoryText; same Inspector slot now drives the LengthOfStayText line.
        [FormerlySerializedAs("_visaStoryText")]
        [SerializeField] private TextMeshProUGUI _lengthOfStayText;

        [SerializeField] private TextMeshProUGUI _enrollmentStoryText;

        [SerializeField] private TextMeshProUGUI _schufaRecord;

        [SerializeField] private TextMeshProUGUI _nationality;
        [SerializeField] private TextMeshProUGUI _placeOfBirth;

        [SerializeField] private TextMeshProUGUI _dateOfBirth;

        public override void Initialize(StudentProfile data)
        {
            base.Initialize(data);
            _studentNameText.text = data.FullName;
            // Budget + Length Stay come from the database now; Enrollment + Schufa stay derived.
            _budgetStoryText.text = data.BudgetText;
            _lengthOfStayText.text = data.LengthOfStayText;
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
