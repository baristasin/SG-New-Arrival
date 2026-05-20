using Game.Scripts.StudentData;
using TMPro;
using UnityEngine;

namespace Game.Scripts.ApartmentHunting
{
    public class StudentPaperBase : HuntingPaperBase<StudentProfile>
    {
        [SerializeField] private TextMeshProUGUI _studentNameText;

        [SerializeField] private TextMeshProUGUI _budgetStoryText;

        [SerializeField] private TextMeshProUGUI _visaStoryText;

        [SerializeField] private TextMeshProUGUI _enrollmentStoryText;

        public override void Initialize(StudentProfile data)
        {
            base.Initialize(data);
            _studentNameText.text = data.FullName;
            _budgetStoryText.text = data.Budget.ToString();
            _visaStoryText.text = data.VisaStatus.ToString();
            _enrollmentStoryText.text = data.IsEnrolled ? "Enrolled" : "Not Enrolled";
        }
    }
}
