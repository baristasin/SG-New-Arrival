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
            // Budget is free-form per student; Length-of-Stay, Enrollment and Schufa all derive
            // from booleans with the longer flavour lines.
            _budgetStoryText.text = data.BudgetText;
            _lengthOfStayText.text = data.IsExchangeStudent
                ? "I'm an exchange student for next 3 months."
                : "I'm thrilled to be starting my master's programme soon!";
            _enrollmentStoryText.text = data.IsEnrolled
                ? "Yes, I'm enrolled. It's so exciting!"
                : "I am not enrolled. I can't live in the dormitories";
            _schufaRecord.text = data.HasPreviousSchufa
                ? "I'm already registered in Germany. I can get a Schufa report."
                : "It's my first time in Germany";
            _nationality.text = data.Nationality;
            _dateOfBirth.text = data.DateOfBirth;
            _placeOfBirth.text = data.PlaceOfBirth;

            if (_studentImage != null && ApartmentHuntingManager.Active != null)
                _studentImage.sprite = ApartmentHuntingManager.Active.GetStudentSpriteForId(data.IdNumber);
        }
    }
}
