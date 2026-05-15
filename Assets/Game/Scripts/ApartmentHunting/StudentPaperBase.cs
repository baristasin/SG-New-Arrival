using TMPro;
using UnityEngine;

namespace Game.Scripts.ApartmentHunting
{
    public class StudentPaperBase : HuntingPaperBase<StudentPaperData>
    {
        [SerializeField] private TextMeshProUGUI _studentNameText;

        [SerializeField] private TextMeshProUGUI _budgetStoryText;
        
        [SerializeField] private TextMeshProUGUI _visaStoryText;
        
        [SerializeField] private TextMeshProUGUI _enrollmentStoryText;
        
        public override void Initialize(StudentPaperData data)
        {
            base.Initialize(data);
            _studentNameText.text = data.StudentName;
            _budgetStoryText.text = data.BudgetStory;
            _visaStoryText.text = data.VisaStory;
            _enrollmentStoryText.text = data.EnrollmentStory;
        }
    }
}